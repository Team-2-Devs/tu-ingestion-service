using Ingestion.Application.Abstractions;

internal sealed class NoopDatabaseInitializer : IDatabaseInitializer
{
  public Task InitializeAsync(CancellationToken ct = default) => Task.CompletedTask;
}
