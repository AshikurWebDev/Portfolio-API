using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Portfolio.Api.Mcp;

/// <summary>
/// MCP prompts: ready-made instructions an AI client can offer as a command
/// (for example a "/write_case_note" slash command in Claude).
/// </summary>
[McpServerPromptType]
public sealed class BlogPrompts
{
    /// <summary>Starts a new case note on a topic.</summary>
    [McpServerPrompt(Name = "write_case_note")]
    [Description("Write a new case note for Ashikur's blog on a topic, following the writing guide, and save it as a draft.")]
    public static string WriteCaseNote(
        [Description("What the post is about, e.g. 'how audit trail rows are written in the ERP'.")] string topic,
        [Description("Facts, notes or code from Ashikur to build the post on (optional).")] string? notes = null) =>
        $"""
        Write a case note for Ashikur Rahman's blog about: {topic}

        Steps:
        1. Call get_writing_guide and follow it exactly, especially the honesty rules.
        2. Call list_posts so you do not repeat an existing topic, and get_post on related posts for tone.
        3. If facts you need are missing, ask Ashikur before writing rather than inventing them.
        4. Draft the post, then call create_draft. Tell Ashikur the slug so he can review and publish it in the admin.

        Ashikur's notes:
        {notes ?? "(none given)"}
        """;
}
