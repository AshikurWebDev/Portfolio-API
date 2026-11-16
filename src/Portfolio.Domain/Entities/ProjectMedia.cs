namespace Portfolio.Domain.Entities;

/// <summary>Media shown on a project card. <see cref="Type"/> is "image", "video" or "fragment".</summary>
public class ProjectMedia
{
    /// <summary>"image", "video" or "fragment".</summary>
    public string Type { get; set; } = "fragment";

    /// <summary>Image or video URL (image/video).</summary>
    public string? Src { get; set; }

    /// <summary>Poster image URL (video).</summary>
    public string? Poster { get; set; }

    /// <summary>Alternative text (image/video).</summary>
    public string? Alt { get; set; }

    /// <summary>Fragment id for live UI built in code (fragment), e.g. "attendance-feed".</summary>
    public string? FragmentId { get; set; }
}
