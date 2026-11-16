using Microsoft.EntityFrameworkCore;
using Portfolio.Application.ApiTokens;
using Portfolio.Application.ApiTokens.Dtos;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Auth;

/// <summary>Default <see cref="IApiTokenService"/>. Tokens look like "pat_..." and are stored as SHA-256 hashes.</summary>
public sealed class ApiTokenService(AppDbContext db, ICurrentUser currentUser, TimeProvider clock) : IApiTokenService
{
    /// <summary>Every personal access token starts with this, so the auth handler can recognise it.</summary>
    public const string TokenPrefix = "pat_";

    /// <inheritdoc />
    public async Task<CreatedApiTokenResponse> CreateAsync(CreateApiTokenRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Sign in first.");
        var secret = TokenHasher.NewSecret(32, TokenPrefix);
        var now = clock.GetUtcNow();
        var token = new ApiToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name.Trim(),
            Prefix = secret[..10],
            TokenHash = TokenHasher.Hash(secret),
            CreatedAt = now,
            ExpiresAt = now.AddDays(request.ExpiresInDays),
        };
        db.ApiTokens.Add(token);
        await db.SaveChangesAsync(ct);
        return new CreatedApiTokenResponse(secret, ToResponse(token));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ApiTokenResponse>> GetMineAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Sign in first.");
        var list = await db.ApiTokens.AsNoTracking().Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
        return list.Select(ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task RevokeAsync(Guid id, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Sign in first.");
        var token = await db.ApiTokens.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct)
                    ?? throw new NotFoundException("API token", id);
        token.RevokedAt ??= clock.GetUtcNow();
        await db.SaveChangesAsync(ct);
    }

    private static ApiTokenResponse ToResponse(ApiToken t) =>
        new(t.Id, t.Name, t.Prefix, t.CreatedAt, t.ExpiresAt, t.LastUsedAt, t.RevokedAt is not null);
}
