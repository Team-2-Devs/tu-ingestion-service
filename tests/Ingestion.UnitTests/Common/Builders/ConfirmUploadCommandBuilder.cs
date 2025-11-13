using Ingestion.Application.Ports.Inbound.Contracts;

namespace Ingestion.UnitTests.Common.Builders;

public sealed class ConfirmUploadCommandBuilder
{
  private Guid _uploadId = Guid.NewGuid();
  private long _bytes = 12345;
  private string _checksum = "sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

  public ConfirmUploadCommandBuilder WithUploadId(Guid uploadId)
  {
    _uploadId = uploadId;
    return this;
  }

  public ConfirmUploadCommandBuilder WithBytes(long bytes)
  {
    _bytes = bytes;
    return this;
  }

  public ConfirmUploadCommandBuilder WithChecksum(string checksum)
  {
    _checksum = checksum;
    return this;
  }

  public ConfirmUploadCommand Build() =>
    new(
      UploadId: _uploadId,
      Bytes: _bytes,
      Checksum: _checksum
    );
}
