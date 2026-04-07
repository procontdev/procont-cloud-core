namespace Domain.Iam;

public sealed class UserRole
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
}
