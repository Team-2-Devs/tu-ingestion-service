using Ingestion.Api.Contracts;
using Ingestion.Application.Ports.Inbound;
using Microsoft.AspNetCore.Mvc;

namespace Ingestion.Api.Controllers;

[ApiController]
[Route("v1/uploads")]
public class UploadsController : ControllerBase
{
  private readonly IStartUpload _startUpload;
  private readonly IConfirmUpload _confirmUpload;

  public UploadsController(IStartUpload startUpload, IConfirmUpload confirmUpload)
  {
    _startUpload = startUpload;
    _confirmUpload = confirmUpload;
  }

  /// <summary>Create a new upload session and get a presigned PUT URL.</summary>
  [HttpPost("start")]
  public async Task<IActionResult> Start([FromBody] StartUploadRequest req, CancellationToken ct)
  {
    var result = await _startUpload.ExecuteAsync(
      new StartUploadCommand(req.Filename, req.ContentType), ct);

    return result switch
    {
      StartUploadResult.Invalid inv => UnprocessableEntity(new { errors = inv.Errors }),
      StartUploadResult.Success ok => Ok(new StartUploadResponse(ok.UploadId, ok.Key, ok.PutUrl, ok.ExpiresAt)),
      _ => Problem(statusCode: 500, detail: "unexpected error")
    };
  }

  /// <summary>Confirm a completed upload and persist metadata</summary> 
  [HttpPost("confirm")]
  public async Task<IActionResult> Confirm([FromBody] ConfirmUploadRequest req, CancellationToken ct)
  {
    var result = await _confirmUpload.ExecuteAsync(
      new ConfirmUploadCommand(req.UploadId, req.Bytes, req.Checksum), ct);

    return result switch
    {
      ConfirmUploadResult.Invalid inv => UnprocessableEntity(new { errors = inv.Errors }),
      ConfirmUploadResult.NotFound nf =>
        NotFound(new
        {
          type = "about:blank",
          title = "upload not found",
          status = 404,
          detail = $"No upload session found for ID {nf.UploadId}."
        }),
      ConfirmUploadResult.Accepted => Accepted(new { status = "Accepted" }),
      _ => Problem(statusCode: 500, detail: "Unexpected error")
    };
  }
}
