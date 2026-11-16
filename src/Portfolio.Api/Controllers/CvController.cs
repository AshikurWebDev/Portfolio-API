using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Portfolio.Application.Cv;
using Portfolio.Application.Cv.Dtos;

namespace Portfolio.Api.Controllers;

/// <summary>Public CV download, used by every "Download CV" button.</summary>
[ApiController]
[Route("api/cv")]
public sealed class CvController(ICvService cv) : ControllerBase
{
    /// <summary>Downloads the CV as a PDF.</summary>
    /// <remarks>
    /// The URL stays the same when a new CV is uploaded, so it is not cached for long: browsers
    /// revalidate with the ETag (a content hash) and get 304 when nothing changed.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(CancellationToken ct)
    {
        var file = await cv.GetFileAsync(ct);
        if (file is null) return Problem(statusCode: StatusCodes.Status404NotFound, title: "No CV has been uploaded yet.");

        Response.Headers.CacheControl = "public, max-age=300, must-revalidate";
        // "attachment" with a clean name: the browser saves "Ashikur-Rahman-CV.pdf" whatever the uploaded file was called.
        return File(file.Data, "application/pdf", "Ashikur-Rahman-CV.pdf", file.UploadedAt, new EntityTagHeaderValue($"\"{file.Version}\""));
    }

    /// <summary>Whether a CV is available, and its download URL. Always 200, so a missing CV is not logged as an error.</summary>
    [HttpGet("info")]
    [Produces("application/json")]
    [ProducesResponseType<PublicCvResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicCvResponse>> Info(CancellationToken ct) =>
        Ok(await cv.GetInfoAsync(ct) is { } info
            ? new PublicCvResponse(true, info.Url, info.UploadedAt)
            : new PublicCvResponse(false, null, null));
}
