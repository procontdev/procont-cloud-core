using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class EfTenantEntityRepository<T>(ProcontDbContext dbContext, ITenantContext tenantContext) : EfRepository<T>(dbContext)
    where T : TenantEntity
{
    public override async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        => await DbContext.Set<T>()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantContext.TenantId)
            .ToListAsync(cancellationToken);

    public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbContext.Set<T>()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantContext.TenantId, cancellationToken);
}
