using Ingestion.Application.Ports.Inbound.Contracts;

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
