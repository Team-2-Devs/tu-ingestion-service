using Ingestion.Application.Ports.Outbound.Contracts;

namespace Ingestion.Application.Ports.Outbound;

/// <summary>Defines the outbound port for communicating with the Storage service to generate presigned URLs.</summary>
public interface IStoragePresignClient
{
  /// <summary>
  /// Requests a presigned PUT URL from the Storage service for uploading an object.
  /// </summary>
  /// <param name="dto">The presign request parameters.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>The generated presigned URL and expiry information.</returns>
  public Task<PresignResponse> PresignPutAsync(PresignPutDto dto, CancellationToken ct = default);
}
