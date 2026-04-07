using Domain.Common;

namespace Application.Common.Interfaces;

public interface IRepository<T> where T : AuditableEntity
{
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
}
