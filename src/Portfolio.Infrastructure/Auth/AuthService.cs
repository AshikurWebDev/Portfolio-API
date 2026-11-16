using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Application.Auth;
using Portfolio.Application.Auth.Dtos;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Identity;
using Portfolio.Infrastructure.Options;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Auth;

/// <summary>
/// Default <see cref="IAuthService"/>, built on ASP.NET Core Identity's UserManager.
///
/// Sign-in flow (both Google and password):
/// 1. Prove who the person is (Google ID token, or Argon2id password check).
/// 2. Check the email is on the admin allowlist (Admin:AllowedEmails).
/// 3. Issue a short-lived JWT access token and a long-lived refresh token.
/// Refresh tokens are rotated: each use revokes the old one and issues a new one.
/// </summary>
public sealed class AuthService(
    UserManager<AppUser> users,
    AppDbContext db,
    ITokenService tokens,
    IGoogleTokenValidator google,
    IOptions<AdminOptions> admin,
    IOptions<JwtOptions> jwt,
    TimeProvider clock,
    ILogger<AuthService> log) : IAuthService
{
    private const string GoogleProvider = "Google";

    /// <inheritdoc />
    public async Task<AuthResult> SignInWithGoogleAsync(GoogleSignInRequest request, CancellationToken ct)
    {
        var identity = await google.ValidateAsync(request.IdToken, ct)
            ?? throw new UnauthorizedException("The Google sign-in could not be verified.");
        if (!identity.EmailVerified)
            throw new UnauthorizedException("This Google account's email is not verified.");
        if (!admin.Value.IsAllowed(identity.Email))
        {
            log.LogWarning("Google sign-in refused for {Email}: not on the admin allowlist.", identity.Email);
            throw new ForbiddenException("This Google account is not allowed to manage the blog.");
        }

        // Find the account by its Google login first, then by email; create it on first sign-in.
        var user = await users.FindByLoginAsync(GoogleProvider, identity.Subject)
                   ?? await users.FindByEmailAsync(identity.Email);
        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = identity.Email,
                Email = identity.Email,
                EmailConfirmed = true,
                DisplayName = identity.Name ?? identity.Email,
                AvatarUrl = identity.Picture,
                CreatedAt = clock.GetUtcNow(),
            };
            EnsureSucceeded(await users.CreateAsync(user)); // no password: Google only
        }
        else
        {
            user.AvatarUrl = identity.Picture ?? user.AvatarUrl;
            if (!string.IsNullOrWhiteSpace(identity.Name)) user.DisplayName = identity.Name;
            await users.UpdateAsync(user);
        }

        var logins = await users.GetLoginsAsync(user);
        if (!logins.Any(l => l.LoginProvider == GoogleProvider))
            EnsureSucceeded(await users.AddLoginAsync(user, new UserLoginInfo(GoogleProvider, identity.Subject, "Google")));

        await EnsureAdminRoleAsync(user);
        return await IssueAsync(user, ct);
    }

    /// <inheritdoc />
    public async Task<AuthResult> SignInWithPasswordAsync(PasswordSignInRequest request, CancellationToken ct)
    {
        const string failure = "Email or password is wrong.";
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null || !admin.Value.IsAllowed(user.Email)) throw new UnauthorizedException(failure);

        // Identity lockout: after 5 wrong passwords the account is locked for 15 minutes.
        if (await users.IsLockedOutAsync(user))
            throw new UnauthorizedException("Too many failed attempts. Try again in a few minutes.");
        if (!await users.CheckPasswordAsync(user, request.Password)) // Argon2id verify (and re-hash if needed)
        {
            await users.AccessFailedAsync(user);
            throw new UnauthorizedException(failure);
        }
        await users.ResetAccessFailedCountAsync(user);

        await EnsureAdminRoleAsync(user);
        return await IssueAsync(user, ct);
    }

    /// <inheritdoc />
    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var hash = TokenHasher.Hash(refreshToken);
        var stored = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        var now = clock.GetUtcNow();
        if (stored is null || !stored.IsActive(now)) throw new UnauthorizedException("Your session has ended. Please sign in again.");

        var user = await users.FindByIdAsync(stored.UserId.ToString());
        if (user is null || !admin.Value.IsAllowed(user.Email)) throw new UnauthorizedException("Your session has ended. Please sign in again.");

        stored.RevokedAt = now; // rotation: this refresh token can never be used again
        return await IssueAsync(user, ct);
    }

    /// <inheritdoc />
    public async Task SignOutAsync(string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken)) return;
        var hash = TokenHasher.Hash(refreshToken);
        var stored = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (stored is null) return;
        stored.RevokedAt ??= clock.GetUtcNow();
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<CurrentUserResponse?> GetUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(userId.ToString());
        return user is null ? null : ToResponse(user, (await users.GetRolesAsync(user)).ToList());
    }

    private async Task<AuthResult> IssueAsync(AppUser user, CancellationToken ct)
    {
        var roles = (await users.GetRolesAsync(user)).ToList();
        var (access, accessExpires) = tokens.CreateAccessToken(user, roles);

        var now = clock.GetUtcNow();
        var refresh = TokenHasher.NewSecret(48);
        var refreshExpires = now.AddDays(jwt.Value.RefreshTokenDays);
        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(refresh),
            CreatedAt = now,
            ExpiresAt = refreshExpires,
        });
        await db.SaveChangesAsync(ct);

        return new AuthResult(new AuthResponse(access, accessExpires, ToResponse(user, roles)), refresh, refreshExpires);
    }

    private async Task EnsureAdminRoleAsync(AppUser user)
    {
        if (!await users.IsInRoleAsync(user, Roles.Admin))
            EnsureSucceeded(await users.AddToRoleAsync(user, Roles.Admin));
    }

    private static CurrentUserResponse ToResponse(AppUser u, IReadOnlyList<string> roles) =>
        new(u.Id, u.Email ?? string.Empty, u.DisplayName, u.AvatarUrl, roles);

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException("Identity operation failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
