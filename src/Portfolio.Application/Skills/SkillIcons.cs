namespace Portfolio.Application.Skills;

/// <summary>
/// The logo keys a skill may use. The website has one SVG per key (web/src/lib/skillIcons.tsx);
/// the API stores only the key, so the database never holds markup. Keep both lists in step.
/// </summary>
public static class SkillIcons
{
    /// <summary>Every accepted key.</summary>
    public static readonly IReadOnlySet<string> Keys = new HashSet<string>(StringComparer.Ordinal)
    {
        // .NET and languages
        "dotnet", "csharp", "blazor", "typescript", "javascript",
        // Frontend
        "react", "angular", "vue", "nextjs", "html5", "css", "tailwind", "bootstrap", "jquery", "redux", "vite",
        // Backend and data
        "nodejs", "express", "postgresql", "mssql", "mysql", "sqlite", "mongodb", "redis",
        // Desktop
        "desktop",
        // Tools and platforms
        "docker", "git", "github", "linux", "nginx", "postman", "swagger", "figma",
        // AI
        "ai", "agent", "prompt", "mcp", "claude", "anthropic", "gemini", "langchain", "ollama", "huggingface", "copilot",
    };

    /// <summary>True when <paramref name="key"/> is null/empty (no logo) or a known key.</summary>
    public static bool IsValid(string? key) => string.IsNullOrEmpty(key) || Keys.Contains(key);
}
