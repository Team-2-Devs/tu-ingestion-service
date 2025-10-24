using Ingestion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ingestion.Infrastructure.Persistence.EFCore;

/// <summary>Entity Framework Core database context for the Ingestion service.</summary>
public sealed class AppDbContext : DbContext
{
  public DbSet<UploadSession> UploadSessions => Set<UploadSession>();

  public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options) { }

  protected override void OnModelCreating(ModelBuilder build)
  {
    var statusConverter = new EnumToStringConverter<UploadStatus>();

    build.Entity<UploadSession>(e =>
    {
      e.HasKey(x => x.Id);

      e.Property(x => x.Key)
       .IsRequired()
       .HasMaxLength(200);

      e.Property(x => x.CreatedAt)
       .IsRequired();

      e.Property(x => x.Status)
       .HasConversion(statusConverter) // store enum as string
       .IsRequired()
       .HasMaxLength(32);

      e.Property(x => x.Bytes);

      e.Property(x => x.Checksum)
       .HasMaxLength(200);
    });
  }

}
