using Domain.Common;

namespace Domain.Iam;

public sealed class Permission : AuditableEntity
{
    public required string Key { get; init; }
    public required string Description { get; set; }
}
