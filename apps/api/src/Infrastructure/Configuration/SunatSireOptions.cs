namespace Infrastructure.Configuration;

public sealed class SunatSireOptions
{
    public const string SectionName = "SunatSire";

    public string BaseUrl { get; set; } = "https://api-sire.sunat.gob.pe";
    public string AuthPath { get; set; } = "/v1/oauth/token";
    public string PropuestasPath { get; set; } = "/v1/contribuyente/propuestas";
    public string ContabilizarPath { get; set; } = "/v1/contribuyente/propuestas/{propuestaId}/contabilizar";
    public string ClientId { get; set; } = string.Empty;
    public string Scope { get; set; } = "sire";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public int RetryBaseDelayMs { get; set; } = 400;
    public bool UseStubResponses { get; set; }
}
