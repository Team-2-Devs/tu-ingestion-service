using Ingestion.Api.Hosting;
using Ingestion.Application.Abstractions;

namespace Ingestion.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddApi(this IServiceCollection services)
  {
    services.AddHttpContextAccessor();
    services.AddScoped<ICorrelationContext, HttpCorrelationContext>();
    
    return services;
  }
}
