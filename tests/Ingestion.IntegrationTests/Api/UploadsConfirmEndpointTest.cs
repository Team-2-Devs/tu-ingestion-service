using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Entities;
using Ingestion.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Ingestion.IntegrationTests.Api;

public sealed class UploadsConfirmEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
  private readonly CustomWebApplicationFactory _factory;
  private readonly HttpClient _client;

  public UploadsConfirmEndpointTests(CustomWebApplicationFactory factory)
  {
    _factory = factory;
    _client = factory.CreateClient();
  }

  private sealed record ConfirmUploadRequest(Guid UploadId, long Bytes, string Checksum);

  // Happy path: existing session, valid bytes and checksum
  [Fact]
  public async Task Confirm_ValidRequest_Returns202_AndPublishesEvent()
  {
    using var scope = _factory.Services.CreateScope();
    var provider = scope.ServiceProvider;

    var repo = (FakeUploadSessionRepository)provider.GetRequiredService<IUploadSessionRepository>();
    var publisher = (FakeEventPublisher)provider.GetRequiredService<IEventPublisher>();

    // Clear any previous state (defensive)
    repo.Store.Clear();
    publisher.Published.Clear();

    // Seed an upload session
    var session = new UploadSession
    {
      Id = Guid.NewGuid(),
      Key = "images/2025/11/09/sample.jpg",
      ContentType = "image/jpeg"
    };
    await repo.AddAsync(session);
    await repo.SaveChangesAsync();

    var req = new ConfirmUploadRequest(
      UploadId: session.Id,
      Bytes: 42,
      Checksum: "sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"
    );

    var response = await _client.PostAsJsonAsync("/v1/uploads/confirm", req);

    response.StatusCode.Should().Be(HttpStatusCode.Accepted);

    // Verify event published
    publisher.Published.Should().HaveCount(1);
    var evt = publisher.Published[0];
    evt.UploadId.Should().Be(session.Id);
    evt.ObjectKey.Should().Be(session.Key);
    evt.Bytes.Should().Be(42);
  }

  // Validation: invalid checksum (422)
  [Fact]
  public async Task Confirm_InvalidChecksum_Returns422()
  {
    var req = new ConfirmUploadRequest(
      UploadId: Guid.NewGuid(),
      Bytes: 42,
      Checksum: "not-a-sha256"
    );

    var response = await _client.PostAsJsonAsync("/v1/uploads/confirm", req);

    response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
  }

  // Not found: valid payload, missing session (404)
  [Fact]
  public async Task Confirm_UnknownUploadId_Returns404()
  {
    var req = new ConfirmUploadRequest(
      UploadId: Guid.NewGuid(),
      Bytes: 42,
      Checksum: "sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"
    );

    var response = await _client.PostAsJsonAsync("/v1/uploads/confirm", req);

    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}

