using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Security;

public sealed class ConfigurationSecretResolver(
    IConfiguration configuration,
    ISecretProvider secretProvider,
    IOptions<SecretManagementOptions> secretOptions,
    ILogger<ConfigurationSecretResolver> logger)
{
    public async Task<string> ResolveRequiredSecretAsync(string configurationPath, string secretName, CancellationToken cancellationToken = default)
    {
        var configuredValue = configuration[configurationPath];
        if (!string.IsNullOrWhiteSpace(configuredValue))
        {
            return configuredValue;
        }

        var secretValue = await secretProvider.GetSecretAsync(secretName, cancellationToken);
        if (!string.IsNullOrWhiteSpace(secretValue))
        {
            logger.LogInformation("secret_resolved backend={Backend} configurationPath={ConfigurationPath} secretName={SecretName}", secretOptions.Value.Backend, configurationPath, secretName);
            return secretValue;
        }

        throw new InvalidOperationException($"No se pudo resolver el secreto requerido '{secretName}' para '{configurationPath}'.");
    }

    public async Task<string?> ResolveOptionalSecretAsync(string configurationPath, string? secretName, CancellationToken cancellationToken = default)
    {
        var configuredValue = configuration[configurationPath];
        if (!string.IsNullOrWhiteSpace(configuredValue))
        {
            return configuredValue;
        }

        if (string.IsNullOrWhiteSpace(secretName))
        {
            return null;
        }

        var secretValue = await secretProvider.GetSecretAsync(secretName, cancellationToken);
        if (!string.IsNullOrWhiteSpace(secretValue))
        {
            logger.LogInformation("secret_resolved backend={Backend} configurationPath={ConfigurationPath} secretName={SecretName}", secretOptions.Value.Backend, configurationPath, secretName);
        }

        return secretValue;
    }
}
