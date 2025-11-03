namespace Ingestion.Application.Abstractions;

/// <summary>
/// Provides access to the current correlation identifier.
/// This abstraction prevents Infrastructure from depending on ASP.NET Core.
/// </summary>
public interface ICorrelationContext
{
  /// <summary>
  /// Retrieves the correlation ID associated with the current request or operation.
  /// </summary>
  public string GetCorrelationId();
}
