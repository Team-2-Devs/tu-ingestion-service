using Ingestion.Application.Ports.Inbound;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Rules;
using Ingestion.Domain.Entities;
using Ingestion.Domain.Factories;

namespace Ingestion.Application.UseCases;

public sealed class StartUpload : IStartUpload
{
  private const int DefaultTtlSec = 300;

  private readonly IStoragePresignClient _storage;
  private readonly IUploadSessionRepository _repo;

  public StartUpload(IStoragePresignClient storage, IUploadSessionRepository repo)
  {
    _storage = storage;
    _repo = repo;
  }

  public async Task<StartUploadResult> ExecuteAsync(StartUploadCommand cmd, CancellationToken ct = default)
  {
    var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

    // Validate
    if (string.IsNullOrWhiteSpace(cmd.Filename))
      errors["filename"] = ["Filename cannot be empty"];

    if (!ContentTypeRules.IsAllowed(cmd.ContentType))
      errors["contentType"] = ["Unsupported MIME type"];

    if (errors.Count > 0)
      return new StartUploadResult.Invalid(errors);

    // If valid
    var key = ObjectKeyFactory.ForImage(cmd.Filename, DateTimeOffset.UtcNow);
    
    var presign = await _storage.PresignPutAsync(
      new PresignPutDto { Key = key, ContentType = cmd.ContentType, TtlSec = DefaultTtlSec }, ct);

    var session = new UploadSession { Id = Guid.NewGuid(), Key = key };
    await _repo.AddAsync(session, ct);
    await _repo.SaveChangesAsync(ct);

    return new StartUploadResult.Success(session.Id, key, presign.Url, presign.ExpiresAt);
  }
}
