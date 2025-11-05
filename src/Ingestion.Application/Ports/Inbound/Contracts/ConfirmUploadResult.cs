namespace Ingestion.Application.Ports.Inbound.Contracts;

/// <summary>Result of an upload confirmation request.</summary>
public abstract record ConfirmUploadResult
{
  /// <summary>Returned when command input validation fails (e.g., missing or invalid fields).</summary>
  public sealed record Invalid(Dictionary<string, string[]> Errors) : ConfirmUploadResult;
  /// <summary>Returned when the specified upload session cannot be found.</summary>
  public sealed record NotFound(Guid UploadId) : ConfirmUploadResult;
  /// <summary>Returned when the upload confirmation is accepted for processing.</summary>
  public sealed record Accepted() : ConfirmUploadResult;
}
