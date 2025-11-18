using Ingestion.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Infrastructure.Adapters.Persistence.EFCore;

public sealed class EfCoreDatabaseInitializer : IDatabaseInitializer
{
  private readonly AppDbContext _db;

  public EfCoreDatabaseInitializer(
      AppDbContext db)
  {
    _db = db;
  }

  public async Task InitializeAsync(
      CancellationToken ct = default)
  {
    // Creates DB file if missing and applies all migrations
    await _db.Database.MigrateAsync(ct);
  }
}
