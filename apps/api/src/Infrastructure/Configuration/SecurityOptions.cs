namespace Infrastructure.Configuration;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public string SecretProvider { get; set; } = "environment";
    public string? SecretReference { get; set; }
}
