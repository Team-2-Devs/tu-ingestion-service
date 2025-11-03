using System.Text.Json;
using Ingestion.Application.Abstractions;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Events;
using Microsoft.Extensions.Options;

namespace Ingestion.Infrastructure.Adapters.Messaging.Redpanda;

/// <summary>
/// Publishes domain events to a Redpanda (Kafka-compatible) message broker.
/// Implements the <see cref="IEventPublisher"/> outbound port.
/// </summary>
public sealed class RedpandaEventPublisher : IEventPublisher
{
  private readonly ICorrelationContext _correlation;
  private readonly MessagingOptions _options;

  public RedpandaEventPublisher(ICorrelationContext correlation, IOptions<MessagingOptions> messagingOptions)
  {
    _correlation = correlation;
    _options = messagingOptions.Value;
  }

  public async Task PublishAsync(ImageUploaded evt, CancellationToken ct)
  {
    var eventName = nameof(ImageUploaded);

    if (!_options.Events.TryGetValue(eventName, out var eventOptions))
      throw new InvalidOperationException($"No configuration found for Messaging:Events:{eventName}.");

    var cid = _correlation.GetCorrelationId();
    var partitionKey = evt.ObjectKey;
    var payload = JsonSerializer.SerializeToUtf8Bytes(evt);

    // TODO: send to broker using eventOptions.Topic
    // headers to include:
    // x-schema = eventOptions.Schema
    // x-producer = _options.Producer
    // x-correlation-id = cid

    await Task.CompletedTask;
  }
}
