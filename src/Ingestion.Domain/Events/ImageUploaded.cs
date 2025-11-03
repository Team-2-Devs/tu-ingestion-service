namespace Ingestion.Domain.Events;

public sealed record ImageUploaded(
  Guid UploadId,
  string ObjectKey,
  string ContentType,
  long Bytes,
  string Checksum,
  DateTimeOffset OccurredAt
  );
