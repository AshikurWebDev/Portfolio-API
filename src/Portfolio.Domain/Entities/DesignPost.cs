using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>
/// One piece of graphic design work shown in the home page gallery.
/// The image is stored in the database as an already-compressed WebP (at most 100 KB),
/// so it survives redeploys and is deleted together with the row.
/// </summary>
public class DesignPost : AuditableEntity
{
    /// <summary>Primary key. Also part of the public image URL.</summary>
    public Guid Id { get; set; }

    /// <summary>Short description, used as the image's alt text.</summary>
    public string Caption { get; set; } = string.Empty;

    /// <summary>Optional link to the original post (Facebook, Behance, ...).</summary>
    public string? Link { get; set; }

    /// <summary>Position in the gallery; lower comes first.</summary>
    public int SortOrder { get; set; }

    /// <summary>Hidden items stay in the admin but are not shown on the site.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Compressed WebP bytes.</summary>
    public byte[] ImageData { get; set; } = [];

    /// <summary>Image width in pixels after compression.</summary>
    public int ImageWidth { get; set; }

    /// <summary>Image height in pixels after compression.</summary>
    public int ImageHeight { get; set; }

    /// <summary>
    /// Short hash of the image bytes. Added to the image URL (?v=...) so browsers can cache
    /// an image forever and still fetch the new one after it is replaced.
    /// </summary>
    public string ImageVersion { get; set; } = string.Empty;
}
