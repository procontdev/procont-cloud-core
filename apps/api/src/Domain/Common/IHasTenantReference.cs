namespace Domain.Common;

public interface IHasTenantReference
{
    Guid TenantId { get; }
}
