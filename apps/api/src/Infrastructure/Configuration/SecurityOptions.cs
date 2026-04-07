namespace Infrastructure.Configuration;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public string SecretProvider { get; set; } = "environment";
    public string? SecretReference { get; set; }
    public bool RotateJwtKeys { get; set; } = true;
    public string JwtActiveKeyVersion { get; set; } = "v1";
    public string[] AllowedJwtKeyVersions { get; set; } = ["v1"];
}
