using FluentAssertions;
using Ingestion.Application.Ports.Inbound.Contracts;
using Ingestion.Application.UseCases;
using Ingestion.Domain.Entities;
using Ingestion.UnitTests.Common.Builders;
using Ingestion.UnitTests.Common.Doubles;

namespace Ingestion.UnitTests.Application.UseCases;

public sealed class ConfirmUploadTests
{
  private const string ValidSha256 = "sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

  // Happy path: validates, finds session, marks uploaded, saves, publishes event
  [Fact]
  public async Task ExecuteAsync_ValidCommand_Accepts_AndPersists_AndPublishesEvent()
  {
    var repo = new FakeUploadSessionRepository();
    var publisher = new FakeEventPublisher();

    var existing = new UploadSession()
    {
      Id = Guid.NewGuid(),
      Key = "images/2025/11/09/sample.jpg",
      ContentType = "image/jpeg",
    };
    await repo.AddAsync(existing);

    var sut = new ConfirmUpload(repo, publisher);

    var cmd = new ConfirmUploadCommandBuilder()
      .WithUploadId(existing.Id)
      .WithBytes(42)
      .WithChecksum(ValidSha256)
      .Build();

    var result = await sut.ExecuteAsync(cmd);

    // Verify result type
    var accepted = result as ConfirmUploadResult.Accepted;
    accepted.Should().NotBeNull();

    // Verify entity updated
    UploadSession? updated = await repo.GetAsync(existing.Id);
    updated.Should().NotBeNull();
    updated!.Status.Should().Be(UploadStatus.Uploaded);
    updated.Bytes.Should().Be(42);
    updated.Checksum.Should().Be(ValidSha256);

    // Verify persistence
    repo.SaveChangesCalls.Should().Be(1);

    // Verify event published
    publisher.Published.Should().HaveCount(1);
    var evt = publisher.Published[0];
    evt.UploadId.Should().Be(existing.Id);
    evt.ObjectKey.Should().Be(existing.Key);
    evt.ContentType.Should().Be(existing.ContentType);
    evt.Bytes.Should().Be(42);
    evt.Checksum.Should().Be(ValidSha256);
  }

  // Validation: bytes <= 0
  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public async Task ExecuteAsync_InvalidBytes_ReturnsInvalid_AndSkipsRepoAndPublisher(long bytes)
  {
    var repo = new FakeUploadSessionRepository();
    var publisher = new FakeEventPublisher();
    var sut = new ConfirmUpload(repo, publisher);

    var cmd = new ConfirmUploadCommandBuilder()
      .WithBytes(bytes)
      .Build();

    var result = await sut.ExecuteAsync(cmd);

    var invalid = result as ConfirmUploadResult.Invalid;
    invalid.Should().NotBeNull();
    invalid!.Errors.Should().ContainKey("bytes");

    repo.SaveChangesCalls.Should().Be(0);
    publisher.Published.Should().BeEmpty();
  }

  // Validation: invalid checksum format
  [Fact]
  public async Task ExecuteAsync_InvalidChecksum_ReturnsInvalid_AndSkipsRepoAndPublisher()
  {
    var repo = new FakeUploadSessionRepository();
    var publisher = new FakeEventPublisher();
    var sut = new ConfirmUpload(repo, publisher);

    var cmd = new ConfirmUploadCommandBuilder()
     .WithChecksum("not-sha256")
     .Build();

    var result = await sut.ExecuteAsync(cmd);

    var invalid = result as ConfirmUploadResult.Invalid;
    invalid.Should().NotBeNull();
    invalid!.Errors.Should().ContainKey("checksum");

    repo.SaveChangesCalls.Should().Be(0);
    publisher.Published.Should().BeEmpty();
  }

  // Not found: valid input but no session
  [Fact]
  public async Task ExecuteAsync_UnknownUploadId_ReturnsNotFound_AndSkipsRepoAndPublisher()
  {
    var repo = new FakeUploadSessionRepository();
    var publisher = new FakeEventPublisher();
    var sut = new ConfirmUpload(repo, publisher);

    var cmd = new ConfirmUploadCommandBuilder()
      .WithUploadId(Guid.NewGuid())
      .Build();

    var result = await sut.ExecuteAsync(cmd);

    var notFound = result as ConfirmUploadResult.NotFound;
    notFound.Should().NotBeNull();

    repo.SaveChangesCalls.Should().Be(0);
    publisher.Published.Should().BeEmpty();
  }


  // Note:
  // Cancellation and exceptional paths are not covered for the semester scope.
}
