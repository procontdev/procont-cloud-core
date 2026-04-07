using Application.Common.Interfaces;

namespace Infrastructure.Auth;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string value) => BCrypt.Net.BCrypt.HashPassword(value, workFactor: 12);

    public bool Verify(string hash, string value) => BCrypt.Net.BCrypt.Verify(value, hash);
}
