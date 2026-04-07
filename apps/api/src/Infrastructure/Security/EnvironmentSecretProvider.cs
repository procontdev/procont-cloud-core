using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Security;

public sealed class EnvironmentSecretProvider(
    IOptions<SecretManagementOptions> options,
    ILogger<EnvironmentSecretProvider> logger) : ISecretProvider
{
    public Task<string?> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        var normalizedName = secretName.Replace('-', '_').Replace(':', '_').ToUpperInvariant();
        var value = Environment.GetEnvironmentVariable($"PROCONT_SECRET__{normalizedName}")
            ?? Environment.GetEnvironmentVariable(normalizedName);

        if (string.IsNullOrWhiteSpace(value) && options.Value.EnforceExternalSecrets)
        {
            logger.LogWarning("secret_missing backend={Backend} secretName={SecretName}", options.Value.Backend, secretName);
        }

        return Task.FromResult<string?>(value);
    }
}
