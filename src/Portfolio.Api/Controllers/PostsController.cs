using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Posts;
using Portfolio.Application.Posts.Dtos;

namespace Portfolio.Api.Controllers;

/// <summary>Public blog endpoints. Only published posts are ever returned here.</summary>
[ApiController]
[Route("api/posts")]
[Produces("application/json")]
public sealed class PostsController(IPostService posts) : ControllerBase
{
    /// <summary>Lists published posts, newest first (no bodies).</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<PostSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PostSummaryResponse>>> GetAll(CancellationToken ct) =>
        Ok(await posts.GetPublishedAsync(ct));

    /// <summary>Gets one published post with its Markdown body.</summary>
    /// <param name="slug">The post's URL part.</param>
    /// <param name="ct">Cancelled when the client disconnects.</param>
    /// <response code="404">No published post has this slug (drafts also return 404).</response>
    [HttpGet("{slug}")]
    [ProducesResponseType<PostDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDetailResponse>> GetBySlug(string slug, CancellationToken ct)
    {
        var post = await posts.GetPublishedBySlugAsync(slug, ct);
        return post is null ? NotFound() : Ok(post);
    }
}
