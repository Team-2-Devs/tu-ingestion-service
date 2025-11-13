using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Events;

namespace Ingestion.UnitTests.Common.Doubles;

public sealed class FakeEventPublisher : IEventPublisher
{
  public List<ImageUploaded> Published { get; } = new List<ImageUploaded>();

  public Task PublishAsync(ImageUploaded evt, CancellationToken ct)
  {
    Published.Add(evt);
    return Task.CompletedTask;
  }
}
