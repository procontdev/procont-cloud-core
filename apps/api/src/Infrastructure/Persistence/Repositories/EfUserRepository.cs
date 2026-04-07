using Application.Common.Interfaces;
using Domain.Iam;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class EfUserRepository(ProcontDbContext dbContext, ITenantContext tenantContext) : EfRepository<User>(dbContext), IUserRepository
{
    public override async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
        => await DbContext.Users.AsNoTracking().Where(x => x.TenantId == tenantContext.TenantId).ToListAsync(cancellationToken);

    public override Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => DbContext.Users.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantContext.TenantId, cancellationToken);

    public Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
        => DbContext.Users.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Email == email, cancellationToken);

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query =
            from userRole in DbContext.UserRoles
            join role in DbContext.Roles on userRole.RoleId equals role.Id
            where userRole.UserId == userId
            select role.Name;

        return await query.Distinct().OrderBy(x => x).ToListAsync(cancellationToken);
    }
}
