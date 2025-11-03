namespace Ingestion.Infrastructure.Adapters.Messaging;

public sealed class MessagingOptions
{
  public string Producer { get; set; } = "ingestion";
  public Dictionary<string, EventOptions> Events { get; init; } = new();
}
