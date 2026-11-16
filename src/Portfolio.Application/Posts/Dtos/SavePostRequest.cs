using System.ComponentModel.DataAnnotations;
using Portfolio.Application.Common;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Posts.Dtos;

/// <summary>
/// Payload for creating or updating a post. New posts are always saved as drafts;
/// publishing is a separate action (POST /api/admin/posts/{id}/publish).
/// </summary>
public sealed record SavePostRequest
{
    /// <summary>URL part. Leave empty when creating to build it from the title.</summary>
    [MaxLength(SlugRules.MaxLength), RegularExpression(SlugRules.Pattern, ErrorMessage = "Use lowercase letters, digits and single hyphens only.")]
    public string? Slug { get; init; }

    /// <summary>Headline.</summary>
    [Required, MaxLength(200)]
    public required string Title { get; init; }

    /// <summary>One or two sentences for lists and search results.</summary>
    [Required, MaxLength(400)]
    public required string Excerpt { get; init; }

    /// <summary>Markdown body. Embeds: ::figure{id=... caption="..."} and ::layer{id=ui|api|logic|data}.</summary>
    [Required, MaxLength(100_000)]
    public required string Body { get; init; }

    /// <summary>Category label, e.g. ".NET Core", "React", "SQL Server &amp; PostgreSQL", "Architecture", "Case notes".</summary>
    [Required, MaxLength(60)]
    public required string Category { get; init; }

    /// <summary>The system layer the post is mainly about.</summary>
    public required SystemLayer Layer { get; init; }

    /// <summary>Tags, lowercase with hyphens.</summary>
    [MaxLength(12)]
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>Figure id for the header (see GET /api/figures), or null.</summary>
    [MaxLength(60)]
    public string? LeadFigure { get; init; }

    /// <summary>Cover image URL, or null.</summary>
    [Url, MaxLength(500)]
    public string? CoverUrl { get; init; }

    /// <summary>Cover alt text; required when a cover URL is set.</summary>
    [MaxLength(200)]
    public string? CoverAlt { get; init; }

    /// <summary>Prefer this post on the home page.</summary>
    public bool Featured { get; init; }
}
