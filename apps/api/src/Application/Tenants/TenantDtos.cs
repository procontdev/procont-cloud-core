using Domain.Tenants;

namespace Application.Tenants;

public sealed record TenantResponse(Guid Id, string Code, string Name, TenantStatus Status, DateTime CreatedAt);
