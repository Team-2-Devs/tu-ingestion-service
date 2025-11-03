using Ingestion.Application.Abstractions;

namespace Ingestion.Api.Hosting;

/// <summary>Retrieves the correlation ID from the current HTTP request context.</summary>
public sealed class HttpCorrelationContext : ICorrelationContext
{
  private readonly IHttpContextAccessor _accessor;

  public HttpCorrelationContext(IHttpContextAccessor accessor)
  {
      _accessor = accessor;
  }

  public string GetCorrelationId()
  {
    var httpContext = _accessor.HttpContext;
    
    return httpContext?.Items["CorrelationId"] as string 
      ?? Guid.NewGuid().ToString("N");
  }
}
