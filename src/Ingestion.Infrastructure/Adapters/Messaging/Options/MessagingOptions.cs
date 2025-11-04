namespace Ingestion.Infrastructure.Adapters.Messaging.Options;

/// <summary>
/// Configuration for the messaging subsystem.
/// Bound to the <c>Messaging</c> section in appsettings.json.
/// </summary>
public sealed class MessagingOptions
{
  public string Producer { get; set; } = "ingestion";
  public KafkaOptions Kafka { get; init; } = new();
  public Dictionary<string, EventOptions> Events { get; init; } = new();
}
