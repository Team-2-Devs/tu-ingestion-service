using Ingestion.Application.Ports.Outbound;
using Ingestion.Application.Ports.Outbound.Contracts;

namespace Ingestion.UnitTests.Common.Doubles;

public sealed class FakeStoragePresignClient : IStoragePresignClient
{
  public StoragePresignPutRequest? LastPutRequest { get; private set; }

  public string UrlToReturn { get; set; } = "http://local/upload";
  public DateTimeOffset ExpiresAtToReturn { get; set; } = DateTimeOffset.Parse("2030-01-01T00:00:00Z");

  public Task<StoragePresignPutResponse> PresignPutAsync(
    StoragePresignPutRequest request,
    CancellationToken ct = default)
  {
    LastPutRequest = request;

    return Task.FromResult(
      new StoragePresignPutResponse(UrlToReturn, ExpiresAtToReturn)
    );
  }
}
