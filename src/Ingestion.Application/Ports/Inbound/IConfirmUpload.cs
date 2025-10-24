namespace Ingestion.Application.Ports.Inbound;

// <summary>Defines the inbound port for confirming completed uploads.</summary>
public interface IConfirmUpload
{
  /// <summary>
  /// Handles a request to confirm that an upload has completed successfully.
  /// </summary>
  /// <param name="cmd">The upload confirmation data.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>A result indicating validation outcome or success status.</returns>
  public Task<ConfirmUploadResult> ExecuteAsync(ConfirmUploadCommand cmd, CancellationToken ct = default);
}

/// <summary>Command data for confirming a completed upload.</summary>
public sealed record ConfirmUploadCommand(Guid UploadId, long Bytes, string Checksum);

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
