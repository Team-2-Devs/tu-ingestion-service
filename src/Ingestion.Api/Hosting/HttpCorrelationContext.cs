using Ingestion.Application.Abstractions;

namespace Ingestion.Api.Hosting;

/// <summary>
/// Implementation of <see cref="ICorrelationContext"/> that retrieves the
/// correlation ID from the current HTTP context, or generates a new one if missing.
/// </summary>
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
