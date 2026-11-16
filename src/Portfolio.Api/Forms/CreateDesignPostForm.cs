using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Forms;

/// <summary>
/// Multipart form for uploading a new design item (POST /api/admin/design-posts).
/// Lives in the Api project because <see cref="IFormFile"/> is an HTTP type; the service
/// only receives the file's stream.
/// </summary>
public sealed class CreateDesignPostForm
{
    /// <summary>The image: JPEG, PNG, WebP or GIF, up to 15 MB. It is converted to WebP of at most 100 KB.</summary>
    [Required]
    public IFormFile File { get; init; } = default!;

    /// <summary>Short description; also the image's alt text.</summary>
    [Required, MaxLength(200)]
    public string Caption { get; init; } = string.Empty;

    /// <summary>Optional http(s) link to the original post.</summary>
    [MaxLength(500)]
    public string? Link { get; init; }
}
