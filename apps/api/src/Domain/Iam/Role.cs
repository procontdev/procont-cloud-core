using Domain.Common;

namespace Domain.Iam;

public sealed class Role : TenantEntity
{
    public required string Name { get; init; }
}
