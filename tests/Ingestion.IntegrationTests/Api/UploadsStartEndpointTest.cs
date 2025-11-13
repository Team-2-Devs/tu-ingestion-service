using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Ingestion.IntegrationTests.Common;

namespace Ingestion.IntegrationTests.Api;

public sealed class UploadsStartEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
  private readonly HttpClient _client;

  public UploadsStartEndpointTests(CustomWebApplicationFactory factory)
  {
    _client = factory.CreateClient();
  }

  private sealed record StartUploadRequest(string Filename, string ContentType);
  private sealed record StartUploadResponse(Guid UploadId, string Key, string PutUrl, DateTimeOffset ExpiresAt);

  // Happy path
  [Fact]
  public async Task Start_ValidRequest_Returns200_WithResponseFields()
  {
    var request = new StartUploadRequest("photo.png", "image/png");

    var response = await _client.PostAsJsonAsync("/v1/uploads/start", request);

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadFromJsonAsync<StartUploadResponse>();
    body.Should().NotBeNull();

    body!.UploadId.Should().NotBe(Guid.Empty);

    body.Key.Should().StartWith("images/");
    body.Key.Should().MatchRegex("^images/\\d{4}/\\d{2}/\\d{2}/[0-9a-fA-F-\\-]{36}\\.(png|jpg|jpeg|webp)$");

    body.PutUrl.Should().Be("http://local/upload");
    body.ExpiresAt.Should().Be(DateTimeOffset.Parse("2030-01-01T00:00:00Z"));
  }

  // Validation: empty filename -> 422
  [Fact]
  public async Task Start_InvalidFilename_Returns422()
  {
    var request = new StartUploadRequest("", "image/jpeg");

    var response = await _client.PostAsJsonAsync("/v1/uploads/start", request);

    response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
  }

  // Validation: unsupported content type -> 422
  [Fact]
  public async Task Start_InvalidContentType_Returns422()
  {
    var request = new StartUploadRequest("test.jpg", "text/plain");

    var response = await _client.PostAsJsonAsync("/v1/uploads/start", request);

    response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
  }
}
