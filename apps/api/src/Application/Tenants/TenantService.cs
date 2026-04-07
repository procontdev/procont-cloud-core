using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Tenants;

namespace Application.Tenants;

public sealed class TenantService(ITenantRepository tenantRepository)
{
    public async Task<TenantResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Tenant no encontrado.");
        return new TenantResponse(tenant.Id, tenant.Code, tenant.Name, tenant.Status, tenant.CreatedAt);
    }
}
