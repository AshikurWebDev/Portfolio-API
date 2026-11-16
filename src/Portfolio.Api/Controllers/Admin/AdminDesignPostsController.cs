using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Api.Forms;
using Portfolio.Application.Design;
using Portfolio.Application.Design.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>
/// Manage the design gallery: upload, edit, reorder, hide and delete items.
/// Uploads are compressed to WebP (at most 100 KB, aspect ratio kept) before they are stored.
/// </summary>
[ApiController]
[Route("api/admin/design-posts")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminDesignPostsController(IDesignPostService design) : ControllerBase
{
    /// <summary>Largest upload accepted. The stored WebP is always much smaller.</summary>
    private const long MaxUploadBytes = 15 * 1024 * 1024;

    /// <summary>Lists all items, hidden ones included, in gallery order.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminDesignPostResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminDesignPostResponse>>> GetAll(CancellationToken ct) =>
        Ok(await design.GetAllAsync(ct));

    /// <summary>Gets one item.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminDesignPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminDesignPostResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(await design.GetAsync(id, ct));

    /// <summary>Uploads a new item (multipart/form-data: file, caption, link).</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxUploadBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes + 64 * 1024)]
    [ProducesResponseType<AdminDesignPostResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminDesignPostResponse>> Create([FromForm] CreateDesignPostForm form, CancellationToken ct)
    {
        await using var stream = form.File.OpenReadStream();
        var created = await design.CreateAsync(stream, form.Caption, form.Link, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>Changes caption, link and visibility.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminDesignPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminDesignPostResponse>> Update(Guid id, UpdateDesignPostRequest request, CancellationToken ct) =>
        Ok(await design.UpdateAsync(id, request, ct));

    /// <summary>Replaces an item's image, keeping its caption and position (multipart/form-data: file).</summary>
    [HttpPut("{id:guid}/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxUploadBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes + 64 * 1024)]
    [ProducesResponseType<AdminDesignPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminDesignPostResponse>> ReplaceImage(Guid id, [FromForm] ReplaceDesignImageForm form, CancellationToken ct)
    {
        await using var stream = form.File.OpenReadStream();
        return Ok(await design.ReplaceImageAsync(id, stream, ct));
    }

    /// <summary>Saves a new gallery order. Send every item id once, first item first.</summary>
    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reorder(ReorderDesignPostsRequest request, CancellationToken ct)
    {
        await design.ReorderAsync(request, ct);
        return NoContent();
    }

    /// <summary>Deletes an item and its image.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await design.DeleteAsync(id, ct);
        return NoContent();
    }
}
