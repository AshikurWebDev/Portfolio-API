namespace Portfolio.Domain.Enums;

/// <summary>Publication state of a blog post.</summary>
public enum PostStatus
{
    /// <summary>Visible only in the admin and through the MCP server. Never returned by public endpoints.</summary>
    Draft,

    /// <summary>Visible on the public blog.</summary>
    Published,
}
