using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Ingestion.Application.Abstractions;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Events;
using Ingestion.Infrastructure.Adapters.Messaging.Options;
using Microsoft.Extensions.Options;

namespace Ingestion.Infrastructure.Adapters.Messaging.Redpanda;

/// <summary>
/// Publishes domain events to a Redpanda (Kafka-compatible) message broker.
/// Implements the <see cref="IEventPublisher"/> outbound port.
/// </summary>
public sealed class RedpandaEventPublisher : IEventPublisher
{
  private readonly ICorrelationContext _correlation;
  private readonly MessagingOptions _messagingOptions;
  private readonly IProducer<string, byte[]> _producer;

  public RedpandaEventPublisher(
    ICorrelationContext correlation, 
    IOptions<MessagingOptions> messagingOptions,
    IProducer<string, byte[]> producer)
  {
    _correlation = correlation;
    _messagingOptions = messagingOptions.Value;
    _producer = producer;
  }

  public async Task PublishAsync(ImageUploaded evt, CancellationToken ct)
  {
    var eventName = nameof(ImageUploaded);

    if (!_messagingOptions.Events.TryGetValue(eventName, out var eventOptions))
      throw new InvalidOperationException($"No configuration found for Messaging:Events:{eventName}.");

    var partitionKey = evt.ObjectKey;
    var payload = JsonSerializer.SerializeToUtf8Bytes(evt);
    var cid = _correlation.GetCorrelationId();

    var message = new Message<string, byte[]>()
    {
      Key = partitionKey,
      Value = payload,
      Headers = new Headers()
      {
        { "x-schema", Encoding.UTF8.GetBytes(eventOptions.Schema) },
        { "x-producer", Encoding.UTF8.GetBytes(_messagingOptions.Producer) },
        { "x-correlation-id", Encoding.UTF8.GetBytes(cid)}
      }
    };

    var result = await _producer.ProduceAsync(eventOptions.Topic, message, ct);
    Console.WriteLine($"[Kafka] Published '{eventName}' to {result.TopicPartitionOffset}");
  }
}
