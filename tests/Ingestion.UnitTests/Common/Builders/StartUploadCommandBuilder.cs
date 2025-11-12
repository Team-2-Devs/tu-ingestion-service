using Ingestion.Application.Ports.Inbound.Contracts;

namespace Ingestion.UnitTests.Common.Builders;

public sealed class StartUploadCommandBuilder
{
  private string _filename = "sample.jpg";
  private string _contentType = "image/jpeg";

  public StartUploadCommandBuilder WithFilename(string filename)
  {
    _filename = filename;
    return this;
  }

  public StartUploadCommandBuilder WithContentType(string contentType)
  {
    _contentType = contentType;
    return this;
  }

  public StartUploadCommand Build() =>
    new StartUploadCommand(
      Filename: _filename,
      ContentType: _contentType
    );
}
