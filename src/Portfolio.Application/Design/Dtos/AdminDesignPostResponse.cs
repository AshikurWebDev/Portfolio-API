namespace Portfolio.Application.Design.Dtos;

/// <summary>A design item as the admin sees it, including hidden items and image details.</summary>
/// <param name="Id">Item id.</param>
/// <param name="Caption">Short description; also the image's alt text.</param>
/// <param name="Link">Optional link to the original post.</param>
/// <param name="IsVisible">False when hidden from the site.</param>
/// <param name="SortOrder">Position in the gallery; lower comes first.</param>
/// <param name="ImageUrl">Relative URL of the stored WebP.</param>
/// <param name="Width">Stored image width in pixels.</param>
/// <param name="Height">Stored image height in pixels.</param>
/// <param name="SizeBytes">Stored image size in bytes (always at most 100 KB).</param>
/// <param name="CreatedAt">When the item was added.</param>
/// <param name="UpdatedAt">When it last changed.</param>
public sealed record AdminDesignPostResponse(
    Guid Id,
    string Caption,
    string? Link,
    bool IsVisible,
    int SortOrder,
    string ImageUrl,
    int Width,
    int Height,
    int SizeBytes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
