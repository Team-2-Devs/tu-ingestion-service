namespace Ingestion.Infrastructure.Adapters.Messaging.Options;

public sealed class KafkaOptions
{
  public string BootstrapServers { get; init; } = string.Empty;
}
