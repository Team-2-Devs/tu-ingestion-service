using Ingestion.Application.Ports.Outbound;
using Ingestion.Application.Ports.Outbound.Contracts;
using Ingestion.Domain.Entities;
using Ingestion.Domain.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ingestion.IntegrationTests.Common;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Test");

    builder.ConfigureServices(services =>
    {
      // Replace IStoragePresignClient (for StartUpload)
      var storageDescriptors = services.Where(d => d.ServiceType == typeof(IStoragePresignClient)).ToList();
      foreach (var d in storageDescriptors)
        services.Remove(d);
      services.AddSingleton<IStoragePresignClient, FakeStoragePresignClient>();

      // Replace IUploadSessionRepository (for both use cases)
      var repoDescriptors = services.Where(d => d.ServiceType == typeof(IUploadSessionRepository)).ToList();
      foreach (var d in repoDescriptors)
        services.Remove(d);
      services.AddSingleton<IUploadSessionRepository, FakeUploadSessionRepository>();

      // Replace IEventPublisher (for ConfirmUpload)
      var publisherDescriptors = services.Where(d => d.ServiceType == typeof(IEventPublisher)).ToList();
      foreach (var d in publisherDescriptors)
        services.Remove(d);
      services.AddSingleton<IEventPublisher, FakeEventPublisher>();
    });
  }
}

// Fake for StartUpload
internal sealed class FakeStoragePresignClient : IStoragePresignClient
{
  public Task<StoragePresignPutResponse> PresignPutAsync(StoragePresignPutRequest req, CancellationToken ct = default)
  {
    return Task.FromResult(new StoragePresignPutResponse(
      "http://local/upload", 
      DateTimeOffset.Parse("2030-01-01T00:00:00Z"))
    );
  }   
}

// Fake for both use cases
internal sealed class FakeUploadSessionRepository : IUploadSessionRepository
{
  public List<UploadSession> Store { get; } = new();

  public Task<UploadSession?> GetAsync(Guid id, CancellationToken ct = default)
  {
    var session = Store.FirstOrDefault(s => s.Id == id);
    return Task.FromResult(session);
  }

  public Task AddAsync(UploadSession session, CancellationToken ct = default)
  {
    Store.Add(session);
    return Task.CompletedTask;
  }

  public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}

// Fake for ConfirmUpload
internal sealed class FakeEventPublisher : IEventPublisher
{
  public List<ImageUploaded> Published { get; } = new();

  public Task PublishAsync(ImageUploaded evt, CancellationToken ct)
  {
    Published.Add(evt);
    return Task.CompletedTask;
  }
}
