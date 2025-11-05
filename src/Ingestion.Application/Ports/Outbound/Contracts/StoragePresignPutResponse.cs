namespace Ingestion.Application.Ports.Outbound.Contracts;

/// <summary>Response containing the generated presigned URL and its expiry time.</summary>
public sealed record StoragePresignPutResponse(string Url, DateTimeOffset ExpiresAt);
