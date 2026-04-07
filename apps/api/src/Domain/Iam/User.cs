using Domain.Common;

namespace Domain.Iam;

public sealed class User : TenantEntity
{
    public required string Email { get; init; }
    public required string PasswordHash { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
}
