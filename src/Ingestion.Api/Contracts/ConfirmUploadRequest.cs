namespace Ingestion.Api.Contracts;

/// <summary>Request payload for confirming a completed upload.</summary>
public sealed record ConfirmUploadRequest(Guid UploadId, long Bytes, string Checksum);
