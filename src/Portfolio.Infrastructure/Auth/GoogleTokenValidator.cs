using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Infrastructure.Options;

namespace Portfolio.Infrastructure.Auth;

/// <summary>
/// Default <see cref="IGoogleTokenValidator"/>. Uses Google's library to check the
/// token's signature against Google's public keys, its expiry, its issuer, and
/// that its audience is this site's OAuth client id.
/// </summary>
public sealed class GoogleTokenValidator(IOptions<GoogleAuthOptions> options, ILogger<GoogleTokenValidator> log) : IGoogleTokenValidator
{
    /// <inheritdoc />
    public async Task<GoogleIdentity?> ValidateAsync(string idToken, CancellationToken ct)
    {
        var clientId = options.Value.ClientId;
        if (string.IsNullOrWhiteSpace(clientId))
        {
            log.LogWarning("Google sign-in attempted but Authentication:Google:ClientId is not configured.");
            return null;
        }

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId],
            });
            return new GoogleIdentity(payload.Subject, payload.Email, payload.EmailVerified, payload.Name, payload.Picture);
        }
        catch (InvalidJwtException ex)
        {
            log.LogInformation("Rejected Google ID token: {Reason}", ex.Message);
            return null;
        }
    }
}
