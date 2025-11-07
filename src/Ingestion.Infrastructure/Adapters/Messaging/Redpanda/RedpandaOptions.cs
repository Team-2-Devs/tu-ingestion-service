namespace Ingestion.Infrastructure.Adapters.Messaging.Redpanda;

public sealed class RedpandaOptions
{
  public string BootstrapServers { get; init; } = string.Empty;
}
