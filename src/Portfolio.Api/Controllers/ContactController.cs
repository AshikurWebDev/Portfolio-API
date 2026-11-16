using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Extensions;
using Portfolio.Application.Contact;
using Portfolio.Application.Contact.Dtos;

namespace Portfolio.Api.Controllers;

/// <summary>The website's contact form.</summary>
[ApiController]
[Route("api/contact")]
public sealed class ContactController(IContactService contact) : ControllerBase
{
    /// <summary>Sends a message. Limited to 5 messages per 10 minutes per IP address.</summary>
    /// <response code="202">Message received.</response>
    /// <response code="400">A field is missing or invalid.</response>
    /// <response code="429">Too many messages; try again later.</response>
    [HttpPost]
    [EnableRateLimiting(RateLimitSetup.Contact)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Send(CreateContactMessageRequest request, CancellationToken ct)
    {
        await contact.SubmitAsync(request, ct);
        return Accepted();
    }
}
