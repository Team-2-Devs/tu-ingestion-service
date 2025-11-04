using Confluent.Kafka;
using Ingestion.Infrastructure.Adapters.Messaging.Options;
using Microsoft.Extensions.Options;

namespace Ingestion.Infrastructure.Adapters.Messaging.Redpanda;

public sealed class RedpandaProducerFactory
{
  private readonly MessagingOptions _options;

  public RedpandaProducerFactory(IOptions<MessagingOptions> messagingOptions)
  {
      _options = messagingOptions.Value;
  }

  public IProducer<string, byte[]> CreateProducer()
  {
    var config = new ProducerConfig()
    {
      BootstrapServers = _options.Kafka.BootstrapServers,
      Acks = Acks.All,
      EnableIdempotence = true,
      MessageSendMaxRetries = 3,
      SocketTimeoutMs = 10000
    };

    return new ProducerBuilder<string, byte[]>(config).Build();
  }

}
