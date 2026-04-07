namespace Domain.Iam;

public sealed class RolePermission
{
    public required Guid RoleId { get; init; }
    public required Guid PermissionId { get; init; }
}
