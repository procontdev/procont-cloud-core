using Application.Accounting;
using Application.Common.Interfaces;
using Domain.Accounting;

namespace Application.Tests;

public sealed class AsientoServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsBalancedEntry()
    {
        var tenantId = Guid.NewGuid();
        var periodo = new PeriodoContable { Id = Guid.NewGuid(), TenantId = tenantId, Anio = 2026, Mes = 4, Estado = PeriodoEstado.Abierto };
        var cuentaDebe = new PlanCuenta { Id = Guid.NewGuid(), TenantId = tenantId, Codigo = "1011", Descripcion = "Caja", Nivel = 4, Tipo = "Activo", Activa = true };
        var cuentaHaber = new PlanCuenta { Id = Guid.NewGuid(), TenantId = tenantId, Codigo = "4011", Descripcion = "Tributos", Nivel = 4, Tipo = "Pasivo", Activa = true };
        var asientoRepo = new FakeRepository<Asiento>();
        var periodoRepo = new FakeRepository<PeriodoContable>(periodo);
        var cuentaRepo = new FakeRepository<PlanCuenta>(cuentaDebe, cuentaHaber);
        var tenantContext = new FakeTenantContext(tenantId);
        var auditLogger = new FakeAuditLogger();
        var service = new AsientoService(asientoRepo, periodoRepo, cuentaRepo, tenantContext, auditLogger);

        var result = await service.CreateAsync(new CreateAsientoRequest(
            periodo.Id,
            new DateOnly(2026, 4, 7),
            "Registro de prueba",
            new[]
            {
                new AsientoDetalleRequest(cuentaDebe.Id, 100m, 0m, null),
                new AsientoDetalleRequest(cuentaHaber.Id, 0m, 100m, null)
            }));

        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal(2, result.Detalles.Count);
        Assert.Single(asientoRepo.Items);
        Assert.Single(auditLogger.Actions);
    }

    private sealed class FakeTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId => tenantId;
        public string TenantCode => "demo";
        public Guid? UserId => Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public string? UserEmail => "admin@demo.local";
        public bool IsAuthenticated => true;
    }

    private sealed class FakeAuditLogger : IActionAuditLogger
    {
        public List<string> Actions { get; } = new();
        public Task LogAsync(string action, string resourceType, string resourceId, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
        {
            Actions.Add(action);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRepository<T>(params T[] seed) : IRepository<T> where T : Domain.Common.AuditableEntity
    {
        public List<T> Items { get; } = seed.ToList();
        public Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) => Task.FromResult((IReadOnlyList<T>)Items.ToList());
        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            Items.Add(entity);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
