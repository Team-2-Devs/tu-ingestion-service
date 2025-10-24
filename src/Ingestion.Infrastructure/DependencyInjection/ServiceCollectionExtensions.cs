using Ingestion.Application.Ports.Outbound;
using Ingestion.Infrastructure.ExternalServices;
using Ingestion.Infrastructure.Persistence.EFCore;
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
    // Database context
    services.AddDbContext<AppDbContext>(o =>
      o.UseSqlite(config.GetConnectionString("IngestionDb") ?? "Data Source=../Ingestion.Infrastructure/Persistence/EFCore/Data/Ingestion.db"));

    // Repository
    services.AddScoped<IUploadSessionRepository, UploadSessionRepository>();

    // HTTP client for Storage service
    services.AddHttpClient<IStoragePresignClient, StoragePresignClient>(c =>
    {
      var baseUrl = config["Storage:BaseUrl"] ?? throw new InvalidOperationException("Storage:BaseUrl not configured");
      c.BaseAddress = new Uri(baseUrl);

      var token = config["Storage:InternalToken"];
      if (!string.IsNullOrWhiteSpace(token))
      {
        c.DefaultRequestHeaders.Add("X-Internal-Token", token);
      }
    });

    return services;
  }
}
