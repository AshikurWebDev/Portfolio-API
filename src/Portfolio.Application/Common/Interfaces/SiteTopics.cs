namespace Portfolio.Application.Common.Interfaces;

/// <summary>
/// Names of the content kinds announced through <see cref="ISiteEvents"/>.
/// The website maps each one to the data it refetches (web/src/lib/live.ts).
/// </summary>
public static class SiteTopics
{
    /// <summary>Design gallery items.</summary>
    public const string Design = "design";

    /// <summary>Skills cards and skills.</summary>
    public const string Skills = "skills";

    /// <summary>The CV PDF.</summary>
    public const string Cv = "cv";

    /// <summary>Published blog posts.</summary>
    public const string Posts = "posts";
}
