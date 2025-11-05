using System.Runtime.InteropServices;
using Ingestion.Application.Ports.Inbound;
using Ingestion.Application.Ports.Inbound.Contracts;
using Ingestion.Application.Ports.Outbound;
using Ingestion.Domain.Events;
using Ingestion.Domain.Rules;

namespace Ingestion.Application.UseCases;

public sealed class ConfirmUpload : IConfirmUpload
{
  private readonly IUploadSessionRepository _repo;
  private readonly IEventPublisher _publisher;

  public ConfirmUpload(IUploadSessionRepository repo, IEventPublisher publisher)
  {
    _repo = repo;
    _publisher = publisher;
  }

  public async Task<ConfirmUploadResult> ExecuteAsync(ConfirmUploadCommand cmd, CancellationToken ct = default)
  {
    var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

    // Validate
    if (cmd.Bytes <= 0)
      errors["bytes"] = ["Must be greater than zero"];

    if (!ChecksumRules.IsValidSha256(cmd.Checksum))
      errors["checksum"] = ["Invalid checksum format (expected sha256:<64 hex>)"];

    if (errors.Count > 0)
      return new ConfirmUploadResult.Invalid(errors);

    var session = await _repo.GetAsync(cmd.UploadId, ct);
    if (session is null)
      return new ConfirmUploadResult.NotFound(cmd.UploadId);

    // If valid
    session.MarkUploaded(cmd.Bytes, cmd.Checksum);
    await _repo.SaveChangesAsync(ct);

    var evt = new ImageUploaded(
      session.Id,
      session.Key,
      session.ContentType,
      cmd.Bytes,
      cmd.Checksum,
      DateTimeOffset.UtcNow
      );

    await _publisher.PublishAsync(evt, ct);

    return new ConfirmUploadResult.Accepted();
  }
}
