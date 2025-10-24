using Ingestion.Application.Ports.Inbound;
using Ingestion.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Ingestion.Application.DependencyInjection;

/// <summary>Dependency injection extensions for registering Application-layer services.</summary>
public static class ServiceCollectionExtensions
{
  /// <summary>Registers all Application-layer services, including use cases, validators, and behaviors.</summary>
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    // Use cases
    services.AddScoped<IStartUpload, StartUpload>();
    services.AddScoped<IConfirmUpload, ConfirmUpload>();

    // Validators
    // behaviors

    return services;
  }
}
