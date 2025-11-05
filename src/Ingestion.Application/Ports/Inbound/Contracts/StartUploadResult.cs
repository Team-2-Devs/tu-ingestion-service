namespace Ingestion.Application.Ports.Inbound.Contracts;

/// <summary>Result of an upload start request.</summary>
public abstract record StartUploadResult
{
  /// <summary>Returned when command input validation fails (e.g., missing or invalid fields).</summary>
  public sealed record Invalid(Dictionary<string, string[]> Errors) : StartUploadResult;
  /// <summary>Returned when the upload session is successfully created and presigned URL generated.</summary>
  public sealed record Success(Guid UploadId, string Key, string PutUrl, DateTimeOffset ExpiresAt) : StartUploadResult;
}
