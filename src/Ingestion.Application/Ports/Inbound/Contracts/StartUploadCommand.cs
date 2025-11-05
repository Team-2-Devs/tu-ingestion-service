namespace Ingestion.Application.Ports.Inbound.Contracts;

/// <summary>Command data for starting a new upload session.</summary>
public sealed record StartUploadCommand(string Filename, string ContentType);
