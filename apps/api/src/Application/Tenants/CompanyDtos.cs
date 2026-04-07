using Domain.Tenants;

namespace Application.Tenants;

public sealed record CompanyResponse(Guid Id, Guid TenantId, string Ruc, string Name, CompanyStatus Status, DateTime CreatedAt, Guid? CreatedBy);

public sealed record CreateCompanyRequest(string Ruc, string Name, CompanyStatus Status);
