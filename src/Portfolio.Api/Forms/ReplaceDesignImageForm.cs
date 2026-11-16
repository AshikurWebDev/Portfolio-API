using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Forms;

/// <summary>Multipart form for swapping a design item's image (PUT /api/admin/design-posts/{id}/image).</summary>
public sealed class ReplaceDesignImageForm
{
    /// <summary>The new image: JPEG, PNG, WebP or GIF, up to 15 MB.</summary>
    [Required]
    public IFormFile File { get; init; } = default!;
}
