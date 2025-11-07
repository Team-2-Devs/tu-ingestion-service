using Ingestion.Application.Ports.Inbound.Contracts;

namespace Ingestion.Application.Ports.Inbound;

/// <summary>Defines the inbound port for confirming completed uploads.</summary>
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
