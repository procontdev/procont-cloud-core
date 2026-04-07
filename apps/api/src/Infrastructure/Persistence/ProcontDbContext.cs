using Domain.Accounting;
using Domain.Common;
using Domain.Iam;
using Domain.Sire;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence;

public sealed class ProcontDbContext(DbContextOptions<ProcontDbContext> options) : DbContext(options)
{
    public const string TenantSettingName = "app.current_tenant";

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<PlanCuenta> PlanCuentas => Set<PlanCuenta>();
    public DbSet<PeriodoContable> PeriodosContables => Set<PeriodoContable>();
    public DbSet<Asiento> Asientos => Set<Asiento>();
    public DbSet<AsientoDetalle> AsientoDetalles => Set<AsientoDetalle>();
    public DbSet<SirePropuesta> SirePropuestas => Set<SirePropuesta>();
    public DbSet<SireContabilizacionResultado> SireContabilizacionResultados => Set<SireContabilizacionResultado>();
    public DbSet<ActionAuditLog> ActionAuditLogs => Set<ActionAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTenant(modelBuilder);
        ConfigureCompany(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigurePermission(modelBuilder);
        ConfigureUserRole(modelBuilder);
        ConfigureRolePermission(modelBuilder);
        ConfigurePlanCuenta(modelBuilder);
        ConfigurePeriodo(modelBuilder);
        ConfigureAsiento(modelBuilder);
        ConfigureAsientoDetalle(modelBuilder);
        ConfigureSirePropuesta(modelBuilder);
        ConfigureSireContabilizacionResultado(modelBuilder);
        ConfigureActionAuditLog(modelBuilder);
    }

    private static void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
            ConfigureAudit(entity);
            entity.HasIndex(x => x.Code).IsUnique();
        });
    }

    private static void ConfigureCompany(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.Ruc).HasColumnName("ruc").HasMaxLength(11).IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => new { x.TenantId, x.Ruc }).IsUnique();
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Key).HasColumnName("key").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(200).IsRequired();
            ConfigureAudit(entity);
            entity.HasIndex(x => x.Key).IsUnique();
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.PermissionId).HasColumnName("permission_id");
            entity.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Permission>().WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePlanCuenta(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlanCuenta>(entity =>
        {
            entity.ToTable("plan_cuentas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Nivel).HasColumnName("nivel").IsRequired();
            entity.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Activa).HasColumnName("activa").IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => new { x.TenantId, x.Codigo }).IsUnique();
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePeriodo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PeriodoContable>(entity =>
        {
            entity.ToTable("periodos_contables");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.Anio).HasColumnName("anio").IsRequired();
            entity.Property(x => x.Mes).HasColumnName("mes").IsRequired();
            entity.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => new { x.TenantId, x.Anio, x.Mes }).IsUnique();
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureAsiento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asiento>(entity =>
        {
            entity.ToTable("asientos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.PeriodoId).HasColumnName("periodo_id").IsRequired();
            entity.Property(x => x.Fecha).HasColumnName("fecha").IsRequired();
            entity.Property(x => x.Glosa).HasColumnName("glosa").HasMaxLength(500).IsRequired();
            entity.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.PeriodoId);
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<PeriodoContable>().WithMany().HasForeignKey(x => x.PeriodoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(x => x.Detalles).WithOne().HasForeignKey(x => x.AsientoId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureAsientoDetalle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsientoDetalle>(entity =>
        {
            entity.ToTable("asiento_detalles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.AsientoId).HasColumnName("asiento_id").IsRequired();
            entity.Property(x => x.CuentaId).HasColumnName("cuenta_id").IsRequired();
            entity.Property(x => x.Debe).HasColumnName("debe").HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.Haber).HasColumnName("haber").HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.CentroCosto).HasColumnName("centro_costo").HasMaxLength(50);
            ConfigureAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.AsientoId);
            entity.HasIndex(x => x.CuentaId);
            entity.HasOne<Asiento>().WithMany(x => x.Detalles).HasForeignKey(x => x.AsientoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<PlanCuenta>().WithMany().HasForeignKey(x => x.CuentaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSirePropuesta(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SirePropuesta>(entity =>
        {
            entity.ToTable("sire_propuestas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.Periodo).HasColumnName("periodo").HasMaxLength(7).IsRequired();
            entity.Property(x => x.ImporteTotal).HasColumnName("importe_total").HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(x => x.GeneratedAtUtc).HasColumnName("generated_at_utc").IsRequired();
            entity.Property(x => x.Source).HasColumnName("source").HasMaxLength(50).IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => new { x.TenantId, x.Periodo, x.Source });
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Resultados).WithOne().HasForeignKey(x => x.PropuestaId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSireContabilizacionResultado(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SireContabilizacionResultado>(entity =>
        {
            entity.ToTable("sire_contabilizacion_resultados");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.PropuestaId).HasColumnName("propuesta_id").IsRequired();
            entity.Property(x => x.AsientoId).HasColumnName("asiento_id");
            entity.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(30).IsRequired();
            entity.Property(x => x.Mensaje).HasColumnName("mensaje").HasMaxLength(500).IsRequired();
            entity.Property(x => x.Observacion).HasColumnName("observacion").HasMaxLength(500);
            entity.Property(x => x.ProcessedAtUtc).HasColumnName("processed_at_utc").IsRequired();
            ConfigureTenantAudit(entity);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.PropuestaId);
            entity.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<SirePropuesta>().WithMany(x => x.Resultados).HasForeignKey(x => x.PropuestaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Asiento>().WithMany().HasForeignKey(x => x.AsientoId).OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureActionAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionAuditLog>(entity =>
        {
            entity.ToTable("action_audit_logs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(x => x.UserEmail).HasColumnName("user_email").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
            entity.Property(x => x.ResourceType).HasColumnName("resource_type").HasMaxLength(100).IsRequired();
            entity.Property(x => x.ResourceId).HasColumnName("resource_id").HasMaxLength(100).IsRequired();
            entity.Property(x => x.TraceId).HasColumnName("trace_id").HasMaxLength(100).IsRequired();
            entity.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();
            entity.Property(x => x.Metadata)
                .HasColumnName("metadata_json")
                .HasColumnType("jsonb")
                .HasConversion(
                    value => System.Text.Json.JsonSerializer.Serialize(value, (System.Text.Json.JsonSerializerOptions?)null),
                    value => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(value, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());
            entity.Property(x => x.Metadata).Metadata.SetValueComparer(new ValueComparer<IReadOnlyDictionary<string, string>>(
                (left, right) => (left ?? new Dictionary<string, string>()).OrderBy(x => x.Key).SequenceEqual((right ?? new Dictionary<string, string>()).OrderBy(x => x.Key)),
                value => value.Aggregate(0, (current, pair) => HashCode.Combine(current, pair.Key.GetHashCode(), pair.Value.GetHashCode())),
                value => new Dictionary<string, string>(value)));
            ConfigureAudit(entity);
            entity.HasIndex(x => x.TenantId);
        });
    }

    private static void ConfigureAudit<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity)
        where T : AuditableEntity
    {
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        entity.Property(x => x.CreatedBy).HasColumnName("created_by");
    }

    private static void ConfigureTenantAudit<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity)
        where T : TenantEntity
    {
        ConfigureAudit(entity);
    }
}
