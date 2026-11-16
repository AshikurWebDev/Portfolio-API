using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Portfolio.Application.Design;
using Portfolio.Application.Design.Dtos;

namespace Portfolio.Api.Controllers;

/// <summary>Public, read-only design gallery for the home page. Managed in the admin.</summary>
[ApiController]
[Route("api/design-posts")]
public sealed class DesignPostsController(IDesignPostService design) : ControllerBase
{
    /// <summary>Lists visible design items in gallery order.</summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType<IReadOnlyList<DesignPostResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DesignPostResponse>>> GetVisible(CancellationToken ct) =>
        Ok(await design.GetVisibleAsync(ct));

    /// <summary>Returns one item's image as WebP.</summary>
    /// <remarks>
    /// Image URLs carry a content hash (?v=...), so the response can be cached for a year:
    /// a replaced image gets a new URL. The ETag lets a browser revalidate cheaply (304).
    /// </remarks>
    [HttpGet("{id:guid}/image")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "image/webp")]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(Guid id, CancellationToken ct)
    {
        var image = await design.GetImageAsync(id, ct);
        if (image is null) return NotFound();

        Response.Headers.CacheControl = "public, max-age=31536000, immutable";
        return File(image.Data, "image/webp", lastModified: null, entityTag: new EntityTagHeaderValue($"\"{image.Version}\""));
    }
}
