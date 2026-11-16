using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Application.Contact;
using Portfolio.Application.Contact.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>Contact form messages for the admin.</summary>
[ApiController]
[Route("api/admin/messages")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminMessagesController(IContactService contact) : ControllerBase
{
    /// <summary>Lists messages, newest first.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ContactMessageResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContactMessageResponse>>> GetAll(CancellationToken ct) =>
        Ok(await contact.GetAllAsync(ct));

    /// <summary>Marks a message as read.</summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        await contact.MarkReadAsync(id, ct);
        return NoContent();
    }
}
