using System.Text;
using Portfolio.Application.Figures;

namespace Portfolio.Api.Mcp;

/// <summary>
/// The house style for blog posts, given to AI agents through the MCP server
/// (tool "get_writing_guide" and prompt "write_case_note"). Edit this text to
/// change how agents write.
/// </summary>
public static class WritingGuide
{
    /// <summary>The full guide as Markdown, including the current figure list.</summary>
    public static string Text
    {
        get
        {
            var sb = new StringBuilder("""
                # Writing guide for Ashikur Rahman's Notes

                ## Who reads this
                Hiring managers and tech leads at Dhaka companies hiring ERP / full-stack (.NET + React) developers,
                and organizations that might commission an ERP or SaaS. They should finish a post believing
                Ashikur builds real systems and explains trade-offs clearly.

                ## Honesty rules (most important)
                - Write only what is true of Ashikur's real work. Known facts: a coaching center ERP in daily use
                  (~110 students, 25 staff) on ASP.NET Core, React and SQL Server, with role-based access,
                  multi-tenancy, audit trail and reporting; ZKTeco fingerprint integration and OAuth 2.0 sign-in
                  are in progress; a land syndicate SaaS in development on PostgreSQL + PostGIS, no live client yet.
                - Never invent numbers, benchmarks, clients, incidents or results. If a detail is unknown, leave it
                  out or say plainly that it is planned or in progress.
                - Sample data in code or figures must look like sample data.

                ## Voice
                - First person, plain and direct English. Short sentences. No hype, no "In today's fast-paced world".
                - Lead with the real problem in the first paragraph, then the choice made and why.
                - Explain trade-offs: what was considered, what was rejected, what is still open.

                ## Structure
                - 600 to 1,200 words. 3 to 6 sections with `##` headings that say what the section is about.
                - Start with 1 to 2 paragraphs of context (no heading). End with "What is still open" or a short rule of thumb.
                - Code blocks name their language: ```csharp, ```sql, ```typescript, ```bash or ```json. Keep them short and correct.
                - Tables are fine for comparisons.

                ## Embeds (each on its own line)
                - `::layer{id=ui|api|logic|data}` marks that the following sections are about that system layer.
                  The post page lights the layer in its side rail. Use 2 to 3 markers per post.
                - `::figure{id=<figure id> caption="One sentence saying what the figure shows."}` inserts a diagram.
                  Use only ids from the list below. Set the post's leadFigure to the most important one.

                ## Metadata
                - layer: the one system layer the post is mainly about (ui, api, logic, data).
                - category: one of ".NET Core", "React", "SQL Server & PostgreSQL", "Architecture", "Case notes".
                - tags: 3 to 6 lowercase-hyphenated tags.
                - excerpt: one or two sentences, under 300 characters, stating the problem and the answer.

                ## Figures available
                """);
            sb.AppendLine();
            foreach (var f in FigureCatalog.All) sb.AppendLine($"- `{f.Id}`: {f.Description}");
            sb.AppendLine();
            sb.AppendLine("New figures need code in the website; if a post needs one that does not exist, describe it in a note at the end of the draft instead of using an unknown id.");
            sb.AppendLine();
            sb.AppendLine("## Publishing");
            sb.AppendLine("Agents can only create and edit drafts. Ashikur reviews and publishes from the blog admin.");
            return sb.ToString();
        }
    }
}
