using Confluent.Kafka;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Infrastructure.Adapters.Messaging.Options;
using Ingestion.Infrastructure.Adapters.Messaging.Redpanda;
using Ingestion.Infrastructure.Adapters.Persistence.EFCore;
using Ingestion.Infrastructure.Adapters.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
    // Bind messaging config
    services.Configure<MessagingOptions>(config.GetSection("Messaging"));
    services.Configure<RedpandaOptions>(config.GetSection("Messaging:Redpanda"));

    // Register Kafka producer with durability settings
    services.AddSingleton<IProducer<string, byte[]>>(sp => 
    { 
      var redpanda = sp.GetRequiredService<IOptions<RedpandaOptions>>().Value;

      var producerConfig = new ProducerConfig()
      {
        BootstrapServers = redpanda.BootstrapServers,
        Acks = Acks.All,
        EnableIdempotence = true,
        MessageSendMaxRetries = 3,
        SocketTimeoutMs = 10000
      };

      return new ProducerBuilder<string, byte[]>(producerConfig).Build();
    });

    // Register event publisher
    services.AddScoped<IEventPublisher, RedpandaEventPublisher>();
  }

  private static void AddPersistence(IServiceCollection services, IConfiguration config)
  {
    // Register SQLite database context
    services.AddDbContext<AppDbContext>(o =>
        o.UseSqlite(config.GetConnectionString("IngestionDb") ?? "Data Source=../../.local/ingestion/ingestion.dev.db"));

    // Register repository
    services.AddScoped<IUploadSessionRepository, UploadSessionRepository>();
  }

  private static void AddStorage(IServiceCollection services, IConfiguration config)
  {
    // Register HTTP client for storage service
    services.AddHttpClient<IStoragePresignClient, StoragePresignClient>(client =>
    {
      var baseUrl = config["Storage:BaseUrl"] ?? throw new InvalidOperationException("Storage:BaseUrl not configured");
      
      client.BaseAddress = new Uri(baseUrl);

      var token = config["Storage:InternalAccess"];
      if (!string.IsNullOrWhiteSpace(token))
        client.DefaultRequestHeaders.Add("x-internal-token", token);
    });
  }
}
