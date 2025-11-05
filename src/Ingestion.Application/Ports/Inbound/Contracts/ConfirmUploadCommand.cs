namespace Ingestion.Application.Ports.Inbound.Contracts;

/// <summary>Command data for confirming a completed upload.</summary>
public sealed record ConfirmUploadCommand(Guid UploadId, long Bytes, string Checksum);
