using Ingestion.Domain.Entities;

namespace Ingestion.Application.Ports.Outbound;

/// <summary>Defines the outbound port for managing upload session persistence.</summary>
public interface IUploadSessionRepository
{
  /// <summary>
  /// Retrieves an existing upload session by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the upload session.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>The matching <see cref="UploadSession"/> if found; otherwise, <c>null</c>.</returns>
  public Task<UploadSession?> GetAsync(Guid id, CancellationToken ct = default);

  /// <summary>
  /// Adds a new upload session to the repository.
  /// </summary>
  /// <param name="session">The upload session entity to add.</param>
  /// <param name="ct">Cancellation token.</param>
  public Task AddAsync(UploadSession session, CancellationToken ct = default);

  /// <summary>
  /// Persists all pending changes to the underlying data store.
  /// </summary>
  /// <param name="ct">Cancellation token.</param>
  public Task SaveChangesAsync(CancellationToken ct = default);
}
