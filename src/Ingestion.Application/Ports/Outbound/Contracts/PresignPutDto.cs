namespace Ingestion.Application.Ports.Outbound.Contracts;

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
