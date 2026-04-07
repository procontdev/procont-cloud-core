namespace Application.Common.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
    string TenantCode { get; }
    Guid? UserId { get; }
    string? UserEmail { get; }
    bool IsAuthenticated { get; }
}
