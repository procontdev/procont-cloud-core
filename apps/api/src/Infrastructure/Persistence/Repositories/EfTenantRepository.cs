using Application.Common.Interfaces;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class EfTenantRepository(ProcontDbContext dbContext) : EfRepository<Tenant>(dbContext), ITenantRepository
{
    public Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => DbContext.Tenants.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
}
