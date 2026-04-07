namespace Infrastructure.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "procont-cloud-core";
    public string Audience { get; set; } = "procont-cloud-clients";
    public string Key { get; set; } = string.Empty;
    public string KeyVersion { get; set; } = "v1";
    public int ExpirationHours { get; set; } = 8;
}
