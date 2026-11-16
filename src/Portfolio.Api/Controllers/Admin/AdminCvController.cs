using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Api.Forms;
using Portfolio.Application.Cv;
using Portfolio.Application.Cv.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>Upload, replace or remove the CV PDF.</summary>
[ApiController]
[Route("api/admin/cv")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminCvController(ICvService cv) : ControllerBase
{
    /// <summary>Details of the current CV, or 204 when none is uploaded.</summary>
    [HttpGet]
    [ProducesResponseType<CvInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        await cv.GetInfoAsync(ct) is { } info ? Ok(info) : NoContent();

    /// <summary>Uploads a CV, replacing the current one (multipart/form-data: file). PDF only, up to 5 MB.</summary>
    [HttpPut]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(CvService.MaxBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = CvService.MaxBytes + 64 * 1024)]
    [ProducesResponseType<CvInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CvInfoResponse>> Upload([FromForm] UploadCvForm form, CancellationToken ct)
    {
        await using var stream = form.File.OpenReadStream();
        return Ok(await cv.UploadAsync(stream, form.File.FileName, ct));
    }

    /// <summary>Removes the CV.</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(CancellationToken ct)
    {
        await cv.DeleteAsync(ct);
        return NoContent();
    }
}
