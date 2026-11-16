using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>
/// The CV (résumé) PDF offered by the "Download CV" buttons. There is at most one row:
/// uploading a new CV replaces it. Stored in the database so it survives redeploys.
/// </summary>
public class CvDocument : AuditableEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Original file name, kept for the admin page only. Downloads use a fixed, clean name.</summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>The PDF bytes.</summary>
    public byte[] Data { get; set; } = [];

    /// <summary>Short content hash, used as the download's ETag so browsers fetch a replaced CV at once.</summary>
    public string Version { get; set; } = string.Empty;
}
