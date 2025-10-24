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

/// <summary>Data transfer object for requesting a presigned PUT URL from the Storage service.</summary>
public sealed record PresignPutDto
{
  /// <summary>The unique object key identifying the file in storage.</summary>
  public required string Key { get; init; }
  /// <summary>The MIME content type of the object to be uploaded.</summary>
  public required string ContentType { get; init; }
  /// <summary>The time-to-live (in seconds) before the presigned URL expires.</summary>
  public required int TtlSec { get; init; }
}

/// <summary>Response containing the generated presigned URL and its expiry time.</summary>
public sealed record PresignResponse(string Url, DateTimeOffset ExpiresAt);

