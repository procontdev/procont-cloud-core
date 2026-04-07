using Domain.Tenants;

namespace Application.Common.Interfaces;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
