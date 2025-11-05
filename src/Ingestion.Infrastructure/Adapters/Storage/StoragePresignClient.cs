using System.Net.Http.Json;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Application.Ports.Outbound.Contracts;

namespace Ingestion.Infrastructure.Adapters.Storage;

/// <summary>HTTP client adapter for communicating with the Storage service to obtain presigned URLs.</summary>
public sealed class StoragePresignClient(HttpClient http) : IStoragePresignClient
{
  /// <summary>Requests a presigned PUT URL from the Storage service’s internal API.</summary>
  public async Task<PresignResponse> PresignPutAsync(PresignPutDto dto, CancellationToken ct = default)
  {
    var response = await http.PostAsJsonAsync("/internal/v1/storage/presign-put", dto, ct);
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadFromJsonAsync<PresignResponse>(cancellationToken: ct);

    return body!;
  }
}
