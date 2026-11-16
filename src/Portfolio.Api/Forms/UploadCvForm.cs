using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Forms;

/// <summary>Multipart form for uploading the CV (PUT /api/admin/cv).</summary>
public sealed class UploadCvForm
{
    /// <summary>The CV as a PDF, up to 5 MB.</summary>
    [Required]
    public IFormFile File { get; init; } = default!;
}
