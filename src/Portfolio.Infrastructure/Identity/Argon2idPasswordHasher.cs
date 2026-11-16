using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Portfolio.Infrastructure.Identity;

/// <summary>
/// Replaces Identity's default PBKDF2 hasher with Argon2id, the algorithm OWASP
/// recommends for password storage. Hashes are stored in the standard PHC string
/// format so the parameters travel with each hash:
/// <c>$argon2id$v=19$m=19456,t=2,p=1$&lt;salt&gt;$&lt;hash&gt;</c>.
/// When the parameters below are raised later, old hashes still verify and are
/// re-hashed automatically on the next successful sign-in (SuccessRehashNeeded).
/// </summary>
public sealed class Argon2idPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    // OWASP minimum for Argon2id: 19 MiB memory, 2 iterations, 1 degree of parallelism.
    private const int MemoryKib = 19456;
    private const int Iterations = 2;
    private const int Parallelism = 1;
    private const int SaltBytes = 16;
    private const int HashBytes = 32;

    /// <inheritdoc />
    public string HashPassword(TUser user, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = Compute(password, salt, MemoryKib, Iterations, Parallelism, HashBytes);
        return $"$argon2id$v=19$m={MemoryKib},t={Iterations},p={Parallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <inheritdoc />
    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        // Expected parts: "", "argon2id", "v=19", "m=..,t=..,p=..", salt, hash
        var parts = hashedPassword.Split('$');
        if (parts.Length != 6 || parts[1] != "argon2id") return PasswordVerificationResult.Failed;

        int m = 0, t = 0, p = 0;
        foreach (var kv in parts[3].Split(','))
        {
            var pair = kv.Split('=');
            if (pair.Length != 2 || !int.TryParse(pair[1], out var v)) return PasswordVerificationResult.Failed;
            switch (pair[0]) { case "m": m = v; break; case "t": t = v; break; case "p": p = v; break; }
        }

        byte[] salt, expected;
        try
        {
            salt = Convert.FromBase64String(parts[4]);
            expected = Convert.FromBase64String(parts[5]);
        }
        catch (FormatException)
        {
            return PasswordVerificationResult.Failed;
        }

        var actual = Compute(providedPassword, salt, m, t, p, expected.Length);
        // Constant-time comparison so timing does not reveal how many bytes matched.
        if (!CryptographicOperations.FixedTimeEquals(actual, expected)) return PasswordVerificationResult.Failed;

        return m == MemoryKib && t == Iterations && p == Parallelism
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.SuccessRehashNeeded;
    }

    private static byte[] Compute(string password, byte[] salt, int memoryKib, int iterations, int parallelism, int length)
    {
        using var argon = new Argon2id(System.Text.Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = memoryKib,
            Iterations = iterations,
            DegreeOfParallelism = parallelism,
        };
        return argon.GetBytes(length);
    }
}
