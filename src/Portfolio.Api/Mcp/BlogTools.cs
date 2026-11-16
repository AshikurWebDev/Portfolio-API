using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Figures;
using Portfolio.Application.Posts;
using Portfolio.Application.Posts.Dtos;
using Portfolio.Domain.Enums;

namespace Portfolio.Api.Mcp;

/// <summary>
/// MCP tools an AI agent can call to research and write blog posts.
/// Agents can read everything and create or edit drafts, but cannot publish or
/// delete: those stay with the human admin. Every call is authenticated with a
/// personal access token (or admin JWT) and runs as that admin user.
///
/// Tool methods receive application services by dependency injection, exactly
/// like controllers do, so the same business rules apply.
/// </summary>
[McpServerToolType]
public sealed class BlogTools
{
    /// <summary>Returns the house style, honesty rules and embed syntax.</summary>
    [McpServerTool(Name = "get_writing_guide", ReadOnly = true)]
    [Description("Read this first. Returns the writing guide for Ashikur Rahman's blog: audience, honesty rules, voice, structure, embed syntax (::layer, ::figure) and the list of available figures.")]
    public static string GetWritingGuide() => WritingGuide.Text;

    /// <summary>Lists posts with their status.</summary>
    [McpServerTool(Name = "list_posts", ReadOnly = true)]
    [Description("Lists blog posts (drafts and published) with slug, title, status, layer, category and last update. Use it to avoid repeating a topic.")]
    public static async Task<IEnumerable<object>> ListPosts(IPostService posts, CancellationToken ct)
    {
        var all = await Guard(() => posts.GetAllForAdminAsync(ct));
        return all.Select(p => new
        {
            p.Slug, p.Title, Status = p.Status.ToString(), Layer = p.Layer.ToString(), p.Category, p.ReadingMinutes, p.UpdatedAt,
        });
    }

    /// <summary>Reads one post with its full Markdown body.</summary>
    [McpServerTool(Name = "get_post", ReadOnly = true)]
    [Description("Gets one post (draft or published) by slug, including its full Markdown body and metadata.")]
    public static async Task<AdminPostResponse> GetPost(
        IPostService posts,
        [Description("The post's slug, e.g. 'tenant-isolation-ef-core'.")] string slug,
        CancellationToken ct) =>
        await Guard(async () => await posts.GetBySlugForAdminAsync(slug, ct) ?? throw new NotFoundException("Post", slug));

    /// <summary>Lists figures that can be embedded.</summary>
    [McpServerTool(Name = "list_figures", ReadOnly = true)]
    [Description("Lists the figure ids that can be used in ::figure{id=...} lines and as leadFigure, with what each shows.")]
    public static IReadOnlyList<FigureInfo> ListFigures() => FigureCatalog.All;

    /// <summary>Creates a new draft.</summary>
    [McpServerTool(Name = "create_draft", Destructive = false)]
    [Description("Creates a new blog post as a DRAFT (never published automatically). Follow get_writing_guide. Returns the saved draft.")]
    public static async Task<AdminPostResponse> CreateDraft(
        IPostService posts,
        [Description("Headline, under 200 characters.")] string title,
        [Description("One or two sentences, under 300 characters.")] string excerpt,
        [Description("Markdown body with optional ::layer and ::figure lines.")] string body,
        [Description("One of: .NET Core, React, SQL Server & PostgreSQL, Architecture, Case notes.")] string category,
        [Description("Main system layer: ui, api, logic or data.")] string layer,
        [Description("3 to 6 lowercase-hyphenated tags.")] string[] tags,
        [Description("Optional slug (lowercase, digits, hyphens). Built from the title when empty.")] string? slug = null,
        [Description("Optional figure id for the post header (see list_figures).")] string? leadFigure = null,
        CancellationToken ct = default)
    {
        return await Guard(() => posts.CreateDraftAsync(new SavePostRequest
        {
            Slug = string.IsNullOrWhiteSpace(slug) ? null : slug,
            Title = title,
            Excerpt = excerpt,
            Body = body,
            Category = category,
            Layer = ParseLayer(layer),
            Tags = tags,
            LeadFigure = string.IsNullOrWhiteSpace(leadFigure) ? null : leadFigure,
        }, ct));
    }

    /// <summary>Edits an existing draft. Published posts cannot be edited by agents.</summary>
    [McpServerTool(Name = "update_draft", Destructive = false, Idempotent = true)]
    [Description("Updates a DRAFT post by slug. Only the fields you pass change. Published posts cannot be edited through MCP; ask Ashikur to unpublish first.")]
    public static async Task<AdminPostResponse> UpdateDraft(
        IPostService posts,
        [Description("Slug of the draft to change.")] string slug,
        [Description("New headline.")] string? title = null,
        [Description("New excerpt.")] string? excerpt = null,
        [Description("New full Markdown body (replaces the old body).")] string? body = null,
        [Description("New category.")] string? category = null,
        [Description("New main layer: ui, api, logic or data.")] string? layer = null,
        [Description("New tags (replaces the old list).")] string[]? tags = null,
        [Description("New lead figure id, or empty string to remove it.")] string? leadFigure = null,
        CancellationToken ct = default)
    {
        return await Guard(async () =>
        {
            var current = await posts.GetBySlugForAdminAsync(slug, ct) ?? throw new NotFoundException("Post", slug);
            if (current.Status != PostStatus.Draft)
                throw new ForbiddenException("This post is published. Agents can only edit drafts.");

            var request = new SavePostRequest
            {
                Slug = current.Slug,
                Title = title ?? current.Title,
                Excerpt = excerpt ?? current.Excerpt,
                Body = body ?? current.Body,
                Category = category ?? current.Category,
                Layer = layer is null ? current.Layer : ParseLayer(layer),
                Tags = tags ?? current.Tags,
                LeadFigure = leadFigure is null ? current.LeadFigure : (leadFigure.Length == 0 ? null : leadFigure),
                CoverUrl = current.CoverUrl,
                CoverAlt = current.CoverAlt,
                Featured = current.Featured,
            };
            return await posts.UpdateAsync(current.Id, request, ct);
        });
    }

    /// <summary>
    /// Runs a tool body and turns the application's own exceptions into McpException,
    /// whose message the SDK passes to the agent. Other exceptions keep the SDK's
    /// generic message so internal details never leak.
    /// </summary>
    private static async Task<T> Guard<T>(Func<Task<T>> body)
    {
        try
        {
            return await body();
        }
        catch (RequestValidationException ex)
        {
            throw new McpException($"Invalid input: {string.Join(" ", ex.Errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")))}");
        }
        catch (Exception ex) when (ex is NotFoundException or ConflictException or ForbiddenException)
        {
            throw new McpException(ex.Message);
        }
    }

    private static SystemLayer ParseLayer(string layer) =>
        Enum.TryParse<SystemLayer>(layer, ignoreCase: true, out var value) && Enum.IsDefined(value)
            ? value
            : throw new McpException("Invalid input: layer must be ui, api, logic or data.");
}
