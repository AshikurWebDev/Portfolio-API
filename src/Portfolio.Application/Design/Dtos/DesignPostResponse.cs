namespace Portfolio.Application.Design.Dtos;

/// <summary>
/// One graphic design item for the public gallery on the home page.
/// The shape matches the web app's <c>DesignPost</c> type (web/src/types/content.ts).
/// </summary>
/// <param name="Id">Item id.</param>
/// <param name="ImageUrl">Relative URL of the WebP image, e.g. "/api/design-posts/{id}/image?v=ab12cd34".</param>
/// <param name="Caption">Short description; also the image's alt text.</param>
/// <param name="Permalink">Optional link to the original post.</param>
/// <param name="CreatedAt">When the item was added.</param>
/// <param name="Width">Image width in pixels (lets the page reserve space before the image loads).</param>
/// <param name="Height">Image height in pixels.</param>
public sealed record DesignPostResponse(
    Guid Id,
    string ImageUrl,
    string Caption,
    string? Permalink,
    DateTimeOffset CreatedAt,
    int Width,
    int Height);
