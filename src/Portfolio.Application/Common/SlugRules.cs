using System.Text.RegularExpressions;

namespace Portfolio.Application.Common;

/// <summary>
/// Rules for URL slugs. The website's router only accepts lowercase letters,
/// digits and hyphens (see web/src/lib/router.tsx), so the API enforces the same.
/// </summary>
public static partial class SlugRules
{
    /// <summary>The pattern a slug must match.</summary>
    public const string Pattern = "^[a-z0-9]+(?:-[a-z0-9]+)*$";

    /// <summary>Maximum slug length.</summary>
    public const int MaxLength = 120;

    [GeneratedRegex(Pattern)]
    private static partial Regex SlugRegex();

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonSlugChars();

    /// <summary>True when <paramref name="slug"/> is a valid slug.</summary>
    public static bool IsValid(string slug) => slug.Length <= MaxLength && SlugRegex().IsMatch(slug);

    /// <summary>Builds a slug from a title, e.g. "SQL Server or PostgreSQL?" becomes "sql-server-or-postgresql".</summary>
    public static string FromTitle(string title)
    {
        var slug = NonSlugChars().Replace(title.ToLowerInvariant(), "-").Trim('-');
        return slug.Length > MaxLength ? slug[..MaxLength].TrimEnd('-') : slug;
    }
}
