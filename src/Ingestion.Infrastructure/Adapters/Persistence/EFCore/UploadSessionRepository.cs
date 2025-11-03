using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Infrastructure.Adapters.Persistence.EFCore;

/// <summary>Entity Framework Core implementation of <see cref="IUploadSessionRepository"/>.</summary>
public sealed class UploadSessionRepository : IUploadSessionRepository
{
  private readonly AppDbContext _db;

  public UploadSessionRepository(AppDbContext db) => _db = db;

  public Task<UploadSession?> GetAsync(Guid id, CancellationToken ct = default) =>
    _db.UploadSessions.FirstOrDefaultAsync(x => x.Id == id, ct);

  public async Task AddAsync(UploadSession session, CancellationToken ct = default)
  {
    await _db.UploadSessions.AddAsync(session, ct);
  }

  public Task SaveChangesAsync(CancellationToken ct = default) =>
    _db.SaveChangesAsync(ct);
}
