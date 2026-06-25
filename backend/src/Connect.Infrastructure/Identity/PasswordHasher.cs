using Connect.Application.Common.Interfaces;

namespace Connect.Infrastructure.Identity;

/// <summary>BCrypt password hashing (per-hash salt, adaptive work factor).</summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
