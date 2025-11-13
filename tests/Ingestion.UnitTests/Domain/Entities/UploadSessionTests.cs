using FluentAssertions;
using Ingestion.Domain.Entities;

namespace Ingestion.UnitTests.Domain.Entities;

public sealed class UploadSessionTests
{
  private const string ValidSha256 = "sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

  [Fact]
  public void NewSession_DefaultsToPending_AndNoBytesOrChecksum()
  {
    var session = new UploadSession()
    {
      Id = Guid.NewGuid(),
      Key = "images/2025/11/09/sample.jpg",
      ContentType = "image/jpeg"
    };

    session.Status.Should().Be(UploadStatus.Pending);
    session.Bytes.Should().BeNull();
    session.Checksum.Should().BeNull();
  }

  [Fact]
  public void MarkUploaded_UpdatesStatusBytesAndChecksum()
  {
    var session = new UploadSession()
    {
      Id = Guid.NewGuid(),
      Key = "images/2025/11/09/sample.jpg",
      ContentType = "image/jpeg"
    };

    session.MarkUploaded(
      bytes: 12345,
      checksum: ValidSha256
      );

    session.Status.Should().Be(UploadStatus.Uploaded);
    session.Bytes.Should().Be(12345);
    session.Checksum.Should().Be(ValidSha256);
  }
}
