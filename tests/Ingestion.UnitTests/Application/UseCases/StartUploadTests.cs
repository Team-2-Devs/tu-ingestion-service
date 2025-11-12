using FluentAssertions;
using Ingestion.Application.Ports.Inbound.Contracts;
using Ingestion.Application.UseCases;
using Ingestion.UnitTests.Common.Builders;
using Ingestion.UnitTests.Common.Doubles;

namespace Ingestion.UnitTests.Application.UseCases;

public sealed class StartUploadTests
{
  // Happy path: validates, presigns PUT, creates session, saves
  [Fact]
  public async Task ExecuteAsync_ValidCommand_ReturnsSuccess_AndPersistsSession()
  {
    var storage = new FakeStoragePresignClient();
    var repo = new FakeUploadSessionRepository();
    var sut = new StartUpload(storage, repo);

    var cmd = new StartUploadCommandBuilder()
      .WithFilename("photo.png")
      .WithContentType("image/png")
      .Build();

    var result = await sut.ExecuteAsync(cmd);

    // Verify result structure (regex avoids exact date assertions in key)
    var success = result as StartUploadResult.Success;      
    success.Should().NotBeNull();
    success!.UploadId.Should().NotBeEmpty();
    success.Key.Should().StartWith("images/");
    success.Key.Should().MatchRegex("^images/\\d{4}/\\d{2}/\\d{2}/[0-9a-fA-F-]{36}\\.(png|jpg|jpeg|webp)$");
    success.PutUrl.Should().Be(storage.UrlToReturn);
    success.ExpiresAt.Should().Be(storage.ExpiresAtToReturn);

    // Outbound call arguments
    storage.LastPutRequest.Should().NotBeNull();
    storage.LastPutRequest!.ContentType.Should().Be("image/png");
    storage.LastPutRequest!.TtlSec.Should().Be(300);
    storage.LastPutRequest!.Key.Should().Be(success.Key);

    // Persistence side effects
    repo.Added.Should().HaveCount(1);
    repo.Added[0].Id.Should().Be(success.UploadId);
    repo.Added[0].Key.Should().Be(success.Key);
    repo.Added[0].ContentType.Should().Be("image/png");
    repo.SaveChangesCalls.Should().Be(1);
  }

  // Validation: empty filename
  [Fact]
  public async Task ExecuteAsync_EmptyFilename_ReturnsInvalid_AndSkipsSideEffects()
  {
    var storage = new FakeStoragePresignClient();
    var repo = new FakeUploadSessionRepository();
    var sut = new StartUpload(storage, repo);

    var cmd = new StartUploadCommandBuilder()
      .WithFilename("")
      .WithContentType("image/jpeg")
      .Build();

    var result = await sut.ExecuteAsync(cmd);

    var invalid = result as StartUploadResult.Invalid;
    invalid.Should().NotBeNull();
    invalid!.Errors.Should().ContainKey("filename");

    storage.LastPutRequest.Should().BeNull();
    repo.Added.Should().BeEmpty();
    repo.SaveChangesCalls.Should().Be(0);
  }

  // Validation: unsupported content type
  [Fact]
  public async Task ExecuteAsync_UnsupportedContentType_ReturnsInvalid_AndSkipsSideEffects()
  {
    var storage = new FakeStoragePresignClient();
    var repo = new FakeUploadSessionRepository();
    var sut = new StartUpload(storage, repo);

    var cmd = new StartUploadCommandBuilder()
      .WithFilename("sample.jpg")
      .WithContentType("text/plain")
      .Build();

    var result = await sut.ExecuteAsync(cmd);

    var invalid = result as StartUploadResult.Invalid;
    invalid.Should().NotBeNull();
    invalid!.Errors.Should().ContainKey("contentType");

    storage.LastPutRequest.Should().BeNull();
    repo.Added.Should().BeEmpty();
    repo.SaveChangesCalls.Should().Be(0);
  }

  // Note:
  // Cancellation and exceptional paths are not covered for the semester scope.
}
