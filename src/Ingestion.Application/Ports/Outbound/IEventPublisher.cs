using Ingestion.Domain.Events;

namespace Ingestion.Application.Ports.Outbound;

/// <summary>
/// Outbound port for publishing domain events to the messaging infrastructure.
/// </summary>
public interface IEventPublisher
{
  /// <summary>
  /// Publishes a domain event asynchronously to the messaging system.
  /// </summary>
  /// <param name="evt">The domain event to publish.</param>
  /// <param name="ct">Optional cancellation token.</param>
  public Task PublishAsync(ImageUploaded evt, CancellationToken ct);
}
