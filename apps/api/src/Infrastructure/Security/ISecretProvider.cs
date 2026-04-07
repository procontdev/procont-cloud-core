namespace Infrastructure.Security;

public interface ISecretProvider
{
    Task<string?> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);
}
