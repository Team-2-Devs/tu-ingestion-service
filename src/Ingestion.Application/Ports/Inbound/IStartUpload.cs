namespace Ingestion.Application.Ports.Inbound;

/// <summary>Defines the inbound port for starting new upload sessions.</summary>
public interface IStartUpload
{
  /// <summary>
  /// Handles a request to start a new upload session and generate a presigned PUT URL.
  /// </summary>
  /// <param name="cmd">The upload initiation parameters.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>A result indicating validation outcome or success status.</returns>
  public Task<StartUploadResult> ExecuteAsync(StartUploadCommand cmd, CancellationToken ct = default);
}

/// <summary>Command data for starting a new upload session.</summary>
public sealed record StartUploadCommand(string Filename, string ContentType);

/// <summary>Result of an upload start request.</summary>
public abstract record StartUploadResult
{
  /// <summary>Returned when command input validation fails (e.g., missing or invalid fields).</summary>
  public sealed record Invalid(Dictionary<string, string[]> Errors) : StartUploadResult;
  /// <summary>Returned when the upload session is successfully created and presigned URL generated.</summary>
  public sealed record Success(Guid UploadId, string Key, string PutUrl, DateTimeOffset ExpiresAt) : StartUploadResult;
}
