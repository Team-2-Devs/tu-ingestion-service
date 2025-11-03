namespace Ingestion.Infrastructure.Adapters.Messaging;

public sealed class EventOptions
{
  public string Topic { get; init; } = default!;
  public string Schema { get; init; } = default!;
}
