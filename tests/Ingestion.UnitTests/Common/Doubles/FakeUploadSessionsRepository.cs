using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Entities;

namespace Ingestion.UnitTests.Common.Doubles;

public sealed class FakeUploadSessionRepository : IUploadSessionRepository
{
  public List<UploadSession> Added { get; } = new List<UploadSession>();
  public int SaveChangesCalls { get; private set; }

  public Task AddAsync(UploadSession session, CancellationToken ct = default)
  {
    Added.Add(session);
    return Task.CompletedTask;
  }

  public Task<UploadSession?> GetAsync(Guid id, CancellationToken ct = default)
  {
    var entity = Added.FirstOrDefault(session => session.Id == id);
    return Task.FromResult(entity);
  }

  public Task SaveChangesAsync(CancellationToken ct = default)
  {
    SaveChangesCalls++;
    return Task.CompletedTask;
  }
}
