using Domain.Accounting;
using Domain.Common;
using Domain.Iam;
using Domain.Sire;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class ProcontDbSeeder(ProcontDbContext dbContext, Auth.BCryptPasswordHasher passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.Tenants.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var tenant = new Tenant
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Code = "demo",
            Name = "Tenant Demo",
            Status = TenantStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        var adminRole = new Role
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            TenantId = tenant.Id,
            Name = "Admin",
            CreatedAt = now,
            UpdatedAt = now
        };

        var manageAccounting = new Permission
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Key = "accounting.manage",
            Description = "Gestion contable",
            CreatedAt = now,
            UpdatedAt = now
        };

        var manageCompanies = new Permission
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333334"),
            Key = "platform.companies.manage",
            Description = "Gestion de companias",
            CreatedAt = now,
            UpdatedAt = now
        };

        var manageSire = new Permission
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333335"),
            Key = "sire.manage",
            Description = "Operacion base SIRE",
            CreatedAt = now,
            UpdatedAt = now
        };

        var adminUser = new User
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            TenantId = tenant.Id,
            Email = "admin@demo.local",
            PasswordHash = passwordHasher.Hash("Procont2026*"),
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        var company = new Company
        {
            TenantId = tenant.Id,
            Ruc = "20100070970",
            Name = "Procont Demo SAC",
            Status = CompanyStatus.Active,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        };

        var cuentaCaja = new PlanCuenta
        {
            TenantId = tenant.Id,
            Codigo = "1011",
            Descripcion = "Caja",
            Nivel = 4,
            Tipo = "Activo",
            Activa = true,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        };

        var cuentaTributos = new PlanCuenta
        {
            TenantId = tenant.Id,
            Codigo = "4011",
            Descripcion = "Tributos por pagar",
            Nivel = 4,
            Tipo = "Pasivo",
            Activa = true,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        };

        var periodo = new PeriodoContable
        {
            TenantId = tenant.Id,
            Anio = 2026,
            Mes = 4,
            Estado = PeriodoEstado.Abierto,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        };

        var sirePropuestaAbril = new SirePropuesta
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555551"),
            TenantId = tenant.Id,
            Periodo = "2026-04",
            ImporteTotal = 1250.50m,
            Estado = SireEstadoPropuesta.Pendiente,
            GeneratedAtUtc = now,
            Source = "bootstrap",
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        };

        var sirePropuestaMarzo = new SirePropuesta
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555552"),
            TenantId = tenant.Id,
            Periodo = "2026-03",
            ImporteTotal = 840.10m,
            Estado = SireEstadoPropuesta.Observada,
            GeneratedAtUtc = now.AddDays(-7),
            Source = "bootstrap",
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        };

        dbContext.Tenants.Add(tenant);
        dbContext.Roles.Add(adminRole);
        dbContext.Permissions.AddRange(manageAccounting, manageCompanies, manageSire);
        dbContext.RolePermissions.AddRange(
            new RolePermission { RoleId = adminRole.Id, PermissionId = manageAccounting.Id },
            new RolePermission { RoleId = adminRole.Id, PermissionId = manageCompanies.Id },
            new RolePermission { RoleId = adminRole.Id, PermissionId = manageSire.Id });
        dbContext.Users.Add(adminUser);
        dbContext.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
        dbContext.Companies.Add(company);
        dbContext.PlanCuentas.AddRange(cuentaCaja, cuentaTributos);
        dbContext.PeriodosContables.Add(periodo);
        dbContext.SirePropuestas.AddRange(sirePropuestaAbril, sirePropuestaMarzo);

        dbContext.ActionAuditLogs.Add(new ActionAuditLog
        {
            TenantId = tenant.Id,
            UserEmail = adminUser.Email,
            Action = "seed.bootstrap",
            ResourceType = "System",
            ResourceId = tenant.Id.ToString(),
            TraceId = "seed",
            OccurredAt = now,
            Metadata = new Dictionary<string, string> { ["source"] = "migration-seed" },
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = adminUser.Id
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
