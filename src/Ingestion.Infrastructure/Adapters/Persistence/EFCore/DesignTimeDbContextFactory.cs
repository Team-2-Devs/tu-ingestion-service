using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ingestion.Infrastructure.Adapters.Persistence.EFCore;

/// <summary>
/// Provides a design-time factory for creating AppDbContext
/// so EF Core migrations can run without full application DI.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

    optionsBuilder.UseSqlite("Data Source=../../../.local/ingestion/Ingestion.dev.db");

    return new AppDbContext(optionsBuilder.Options);
  }
}
