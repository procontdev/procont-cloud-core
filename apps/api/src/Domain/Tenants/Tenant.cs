using Domain.Common;

namespace Domain.Tenants;

public sealed class Tenant : AuditableEntity
{
    public required string Code { get; init; }
    public required string Name { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;
}
