namespace Ingestion.Application.Abstractions;

public interface IDatabaseInitializer
{
  Task InitializeAsync(
      CancellationToken ct = default);
}
