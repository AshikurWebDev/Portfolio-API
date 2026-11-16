using System.Text.RegularExpressions;

namespace Portfolio.Application.Common;

/// <summary>Estimates reading time for a Markdown body.</summary>
public static partial class ReadingTime
{
    /// <summary>Average reading speed for technical text, in words per minute.</summary>
    private const int WordsPerMinute = 220;

    [GeneratedRegex(@"\S+")]
    private static partial Regex Word();

    /// <summary>Minutes to read <paramref name="markdown"/>, at least 1. Code blocks count like prose.</summary>
    public static int Estimate(string markdown)
    {
        var words = Word().Matches(markdown).Count;
        return Math.Max(1, (int)Math.Round(words / (double)WordsPerMinute));
    }
}
