using Domain.Common;

namespace Domain.Tenants;

public sealed class Company : TenantEntity
{
    public required string Ruc { get; init; }
    public required string Name { get; set; }
    public CompanyStatus Status { get; set; } = CompanyStatus.Active;
}
