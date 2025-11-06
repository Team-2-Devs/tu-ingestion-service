using System.Diagnostics;

namespace Ingestion.Api.Hosting;

public sealed class CorrelationMiddleware
{
  private const string HeaderName = "X-Correlation-ID";
  private readonly RequestDelegate _next;

  public CorrelationMiddleware(RequestDelegate next) =>_next = next;

  public async Task InvokeAsync(HttpContext context)
  {
    var header = context.Request.Headers[HeaderName].ToString();

    var correlationId = string.IsNullOrWhiteSpace(header)
      ? Guid.NewGuid().ToString("N")
      : header;

    context.Items["CorrelationId"] = correlationId;

    context.Response.OnStarting(() =>
    {
      context.Response.Headers[HeaderName] = correlationId;
      return Task.CompletedTask;
    });

    await _next(context);
  }
}
