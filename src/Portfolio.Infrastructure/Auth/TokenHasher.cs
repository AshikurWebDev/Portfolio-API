using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Infrastructure.Auth;

/// <summary>
/// Creates random secrets and hashes them for storage. Refresh tokens and API
/// tokens are long random values, so a fast SHA-256 hash is enough (unlike
/// passwords, which need a slow hash such as Argon2id).
/// </summary>
public static class TokenHasher
{
    /// <summary>A URL-safe random secret with <paramref name="bytes"/> bytes of entropy, with an optional prefix.</summary>
    public static string NewSecret(int bytes = 32, string prefix = "") =>
        prefix + Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>Base64 SHA-256 of the secret, as stored in the database.</summary>
    public static string Hash(string secret) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));
}
