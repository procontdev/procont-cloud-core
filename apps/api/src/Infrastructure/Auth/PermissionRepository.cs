using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;

public sealed class PermissionRepository(Persistence.ProcontDbContext dbContext) : IPermissionRepository
{
    public async Task<IReadOnlyList<string>> GetPermissionKeysByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query =
            from userRole in dbContext.UserRoles
            join rolePermission in dbContext.RolePermissions on userRole.RoleId equals rolePermission.RoleId
            join permission in dbContext.Permissions on rolePermission.PermissionId equals permission.Id
            where userRole.UserId == userId
            select permission.Key;

        return await query.Distinct().OrderBy(x => x).ToListAsync(cancellationToken);
    }
}
