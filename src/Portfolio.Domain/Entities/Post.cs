using Portfolio.Domain.Common;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

/// <summary>
/// A blog post ("Note"). The body is Markdown and may contain embed lines such as
/// <c>::figure{id=zkteco-sync caption="..."}</c> and <c>::layer{id=data}</c>,
/// which the website turns into diagrams and 3D figures.
/// </summary>
public class Post : AuditableEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>URL part, e.g. "tenant-isolation-ef-core". Unique; lowercase letters, digits and hyphens only.</summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>Headline shown on the blog and in the browser tab.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>One or two sentences shown in lists and as the meta description.</summary>
    public string Excerpt { get; set; } = string.Empty;

    /// <summary>Markdown body with optional embed lines.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>Category label, e.g. ".NET Core" or "Case notes".</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>The system layer the post is mainly about.</summary>
    public SystemLayer Layer { get; set; }

    /// <summary>Free-form tags, stored as a PostgreSQL text array.</summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>Optional figure id shown in the post header.</summary>
    public string? LeadFigure { get; set; }

    /// <summary>Optional cover image URL.</summary>
    public string? CoverUrl { get; set; }

    /// <summary>Alt text for the cover image.</summary>
    public string? CoverAlt { get; set; }

    /// <summary>Featured posts are preferred on the home page.</summary>
    public bool Featured { get; set; }

    /// <summary>Draft or published.</summary>
    public PostStatus Status { get; set; } = PostStatus.Draft;

    /// <summary>Publication date shown to readers. Set when the post is first published.</summary>
    public DateOnly? PublishedOn { get; set; }

    /// <summary>Estimated reading time, recalculated from the body on every save.</summary>
    public int ReadingMinutes { get; set; }

    /// <summary>Id of the user who created the post (an Identity user).</summary>
    public Guid? AuthorId { get; set; }
}
