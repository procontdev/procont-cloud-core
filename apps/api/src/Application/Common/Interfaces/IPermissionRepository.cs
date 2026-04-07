namespace Application.Common.Interfaces;

public interface IPermissionRepository
{
    Task<IReadOnlyList<string>> GetPermissionKeysByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
