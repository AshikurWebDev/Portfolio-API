using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Application.Posts;
using Portfolio.Application.Posts.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>Blog management for the admin. Every action requires the Admin role.</summary>
[ApiController]
[Route("api/admin/posts")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminPostsController(IPostService posts) : ControllerBase
{
    /// <summary>Lists all posts, drafts included.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminPostResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminPostResponse>>> GetAll(CancellationToken ct) =>
        Ok(await posts.GetAllForAdminAsync(ct));

    /// <summary>Gets one post by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminPostResponse>> GetById(Guid id, CancellationToken ct) =>
        Ok(await posts.GetByIdAsync(id, ct));

    /// <summary>Creates a draft.</summary>
    /// <response code="201">Draft created; Location points to it.</response>
    /// <response code="409">The slug is already used.</response>
    [HttpPost]
    [ProducesResponseType<AdminPostResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminPostResponse>> Create(SavePostRequest request, CancellationToken ct)
    {
        var post = await posts.CreateDraftAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    /// <summary>Updates a post's content (status unchanged).</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminPostResponse>> Update(Guid id, SavePostRequest request, CancellationToken ct) =>
        Ok(await posts.UpdateAsync(id, request, ct));

    /// <summary>Publishes a post on the public blog.</summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType<AdminPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminPostResponse>> Publish(Guid id, CancellationToken ct) =>
        Ok(await posts.PublishAsync(id, ct));

    /// <summary>Takes a post off the public blog (back to draft).</summary>
    [HttpPost("{id:guid}/unpublish")]
    [ProducesResponseType<AdminPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminPostResponse>> Unpublish(Guid id, CancellationToken ct) =>
        Ok(await posts.UnpublishAsync(id, ct));

    /// <summary>Deletes a post permanently.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await posts.DeleteAsync(id, ct);
        return NoContent();
    }
}
