namespace Ingestion.Api.Contracts;

/// <summary>Request payload for starting a new upload.</summary>
public sealed record StartUploadRequest(string Filename, string ContentType);
