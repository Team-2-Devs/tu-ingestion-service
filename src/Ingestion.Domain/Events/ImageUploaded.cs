namespace Ingestion.Domain.Events;

/// <summary>
/// Represents a domain event raised when an image has been successfully uploaded.
/// </summary>
public sealed record ImageUploaded(
  Guid UploadId,
  string ObjectKey,
  long Bytes,
  string Checksum,
  DateTimeOffset OccurredAt
  );
