using Ingestion.Application.Ports.Outbound;
using Ingestion.Infrastructure.Adapters.Messaging.Options;
using Ingestion.Infrastructure.Adapters.Messaging.Redpanda;
using Ingestion.Infrastructure.Adapters.Persistence.EFCore;
using Ingestion.Infrastructure.Adapters.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ingestion.Infrastructure.DependencyInjection;

/// <summary>Dependency injection extensions for Infrastructure (database, repositories, and external clients).</summary>
public static class ServiceCollectionExtensions
{
  /// <summary>Registers Infrastructure services including DbContext, repositories, and HTTP clients.</summary>
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
  {
    AddMessaging(services, config);
    AddPersistence(services, config);
    AddStorage(services, config);

    return services;
  }

  private static void AddMessaging(IServiceCollection services, IConfiguration config)
  {
    // Messaging configuration
    services.Configure<MessagingOptions>(config.GetSection("Messaging"));

    // Event publisher
    services.AddSingleton<IEventPublisher, RedpandaEventPublisher>();
  }

  private static void AddPersistence(IServiceCollection services, IConfiguration config)
  {
    // Database context
    services.AddDbContext<AppDbContext>(o =>
        o.UseSqlite(config.GetConnectionString("IngestionDb") ?? "Data Source=../../.local/ingestion/ingestion.dev.db"));

    // Repository
    services.AddScoped<IUploadSessionRepository, UploadSessionRepository>();
  }

  private static void AddStorage(IServiceCollection services, IConfiguration config)
  {
    // HTTP client for Storage service
    services.AddHttpClient<IStoragePresignClient, StoragePresignClient>(client =>
    {
      var baseUrl = config["Storage:BaseUrl"] ?? throw new InvalidOperationException("Storage:BaseUrl not configured");
      
      client.BaseAddress = new Uri(baseUrl);

      var token = config["Storage:InternalAccess"];
      if (!string.IsNullOrWhiteSpace(token))
        client.DefaultRequestHeaders.Add("X-Internal-Token", token);
    });
  }
}
