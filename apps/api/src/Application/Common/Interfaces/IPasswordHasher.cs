namespace Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string value);
    bool Verify(string hashedValue, string plainValue);
}
