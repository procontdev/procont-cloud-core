using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EfRepository<T>(ProcontDbContext dbContext) : IRepository<T> where T : AuditableEntity
{
    protected readonly ProcontDbContext DbContext = dbContext;

    public virtual async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        => await DbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
