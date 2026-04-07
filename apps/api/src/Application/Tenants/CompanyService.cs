using Application.Common.Interfaces;
using Domain.Tenants;

namespace Application.Tenants;

public sealed class CompanyService(
    IRepository<Company> companyRepository,
    ITenantContext tenantContext,
    IActionAuditLogger actionAuditLogger)
{
    public async Task<IReadOnlyList<CompanyResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var companies = await companyRepository.ListAsync(cancellationToken);
        return companies.Select(Map).ToList();
    }

    public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Company
        {
            TenantId = tenantContext.TenantId,
            Ruc = request.Ruc,
            Name = request.Name,
            Status = request.Status,
            CreatedBy = tenantContext.UserId
        };

        await companyRepository.AddAsync(entity, cancellationToken);
        await actionAuditLogger.LogAsync("company.create", nameof(Company), entity.Id.ToString(), new Dictionary<string, string>
        {
            ["ruc"] = entity.Ruc
        }, cancellationToken);

        return Map(entity);
    }

    private static CompanyResponse Map(Company company) => new(company.Id, company.TenantId, company.Ruc, company.Name, company.Status, company.CreatedAt, company.CreatedBy);
}
