namespace Ingestion.Api.Contracts;

/// <summary>Response containing upload session details and a presigned PUT URL.</summary>
public sealed record StartUploadResponse(Guid UploadId, string Key, string PutUrl, DateTimeOffset ExpiresAt);
