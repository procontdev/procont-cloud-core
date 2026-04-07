namespace Infrastructure.Tenancy;

public sealed class TenantResolutionOptions
{
    public string HeaderName { get; set; } = "X-Tenant-Code";
    public string QueryKey { get; set; } = "tenant";
    public bool AllowSubdomainResolution { get; set; } = true;
}
