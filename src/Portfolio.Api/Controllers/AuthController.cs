using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Extensions;
using Portfolio.Application.Auth;
using Portfolio.Application.Auth.Dtos;
using Portfolio.Application.Common.Interfaces;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Admin sign-in. Returns a short-lived access token in the body and sets the
/// refresh token as an HttpOnly cookie (scoped to /api/auth) that scripts cannot read.
/// There is no registration endpoint: only allowlisted emails can sign in.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[EnableRateLimiting(RateLimitSetup.Auth)]
public sealed class AuthController(IAuthService auth, ICurrentUser currentUser, IWebHostEnvironment env) : ControllerBase
{
    private const string RefreshCookie = "portfolio_rt";

    /// <summary>Signs in with the ID token from the Google Sign-In button.</summary>
    /// <response code="401">The Google token is invalid or expired.</response>
    /// <response code="403">The Google account is not on the admin allowlist.</response>
    [HttpPost("google")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AuthResponse>> Google(GoogleSignInRequest request, CancellationToken ct) =>
        Ok(WithCookie(await auth.SignInWithGoogleAsync(request, ct)));

    /// <summary>Signs in with email and password.</summary>
    /// <response code="401">Wrong email or password, or the account is locked.</response>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(PasswordSignInRequest request, CancellationToken ct) =>
        Ok(WithCookie(await auth.SignInWithPasswordAsync(request, ct)));

    /// <summary>Gets a new access token using the refresh cookie. Rotates the cookie.</summary>
    /// <response code="401">No valid refresh cookie; sign in again.</response>
    [HttpPost("refresh")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookie, out var token) || string.IsNullOrEmpty(token))
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized", detail: "Please sign in.");
        return Ok(WithCookie(await auth.RefreshAsync(token, ct)));
    }

    /// <summary>Signs out: revokes the refresh token and clears the cookie.</summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        Request.Cookies.TryGetValue(RefreshCookie, out var token);
        await auth.SignOutAsync(token, ct);
        Response.Cookies.Delete(RefreshCookie, CookieOptions(DateTimeOffset.UnixEpoch));
        return NoContent();
    }

    /// <summary>The signed-in user.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<CurrentUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUserResponse>> Me(CancellationToken ct)
    {
        var user = currentUser.UserId is { } id ? await auth.GetUserAsync(id, ct) : null;
        return user is null ? Unauthorized() : Ok(user);
    }

    private AuthResponse WithCookie(AuthResult result)
    {
        Response.Cookies.Append(RefreshCookie, result.RefreshToken, CookieOptions(result.RefreshExpiresAt));
        return result.Response;
    }

    /// <summary>
    /// HttpOnly: JavaScript cannot read it (protects against XSS token theft).
    /// SameSite=Strict: not sent on requests started by other sites (protects against CSRF).
    /// Secure outside Development, where the site runs over HTTPS.
    /// </summary>
    private CookieOptions CookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        Path = "/api/auth",
        Expires = expires,
        IsEssential = true,
    };
}
