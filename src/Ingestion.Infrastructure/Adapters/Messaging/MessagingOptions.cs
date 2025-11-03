namespace Ingestion.Infrastructure.Adapters.Messaging;

/// <summary>
/// Configuration for the messaging subsystem.
/// Bound to the <c>Messaging</c> section in appsettings.json.
/// </summary>
public sealed class MessagingOptions
{
  public string Producer { get; set; } = "ingestion";
  public Dictionary<string, EventOptions> Events { get; init; } = new();
}
