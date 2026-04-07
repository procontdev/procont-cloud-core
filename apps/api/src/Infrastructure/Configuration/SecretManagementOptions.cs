namespace Infrastructure.Configuration;

public sealed class SecretManagementOptions
{
    public const string SectionName = "SecretManagement";

    public string Environment { get; set; } = "dev";
    public string Backend { get; set; } = "environment";
    public bool EnforceExternalSecrets { get; set; }
    public bool RequireJwtKeyRotation { get; set; } = true;
    public string JwtKeySecretName { get; set; } = "jwt-signing-key";
    public string? JwtKeyVersion { get; set; }
    public string? SunatClientSecretName { get; set; } = "sunat-client-secret";
    public string? SunatUsernameSecretName { get; set; } = "sunat-username";
    public string? SunatPasswordSecretName { get; set; } = "sunat-password";
}
