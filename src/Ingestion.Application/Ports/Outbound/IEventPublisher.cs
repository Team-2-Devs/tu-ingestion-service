using Ingestion.Domain.Events;

namespace Ingestion.Application.Ports.Outbound;

/// <summary>
/// Defines an outbound port for publishing domain events to external messaging infrastructure.
/// </summary>
public interface IEventPublisher
{
  /// <summary>
  /// Publishes a domain event asynchronously to the configured message broker.
  /// </summary>
  /// <param name="evt">The domain event instance to publish.</param>
  /// <param name="ct">A cancellation token for the asynchronous operation.</param>
  public Task PublishAsync(ImageUploaded evt, CancellationToken ct);
}
