namespace Ingestion.Domain.Entities;

public enum UploadStatus
{
  Pending,
  Uploaded
}

public sealed class UploadSession
{
  public Guid Id { get; init; }

  public required string Key { get; init; }
  public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

  public UploadStatus Status { get; private set; } = UploadStatus.Pending;
  public long? Bytes { get; private set; }
  public string? Checksum { get; private set; }

  public void MarkUploaded(long bytes, string checksum)
  {
    Status = UploadStatus.Uploaded;
    Bytes = bytes;
    Checksum = checksum;
  }
}
