using Domain.Accounting;

namespace Domain.Tests;

public sealed class AccountingRulesTests
{
    [Fact]
    public void EnsureBalanced_Throws_WhenDebeAndHaberDiffer()
    {
        var detalles = new[]
        {
            new AsientoDetalle { AsientoId = Guid.NewGuid(), CuentaId = Guid.NewGuid(), Debe = 100m, Haber = 0m },
            new AsientoDetalle { AsientoId = Guid.NewGuid(), CuentaId = Guid.NewGuid(), Debe = 0m, Haber = 99m }
        };

        Assert.Throws<InvalidOperationException>(() => AccountingRules.EnsureBalanced(detalles));
    }

    [Fact]
    public void EnsurePeriodoAbierto_Throws_WhenPeriodoCerrado()
    {
        var periodo = new PeriodoContable { TenantId = Guid.NewGuid(), Anio = 2026, Mes = 4, Estado = PeriodoEstado.Cerrado };

        Assert.Throws<InvalidOperationException>(() => AccountingRules.EnsurePeriodoAbierto(periodo));
    }

    [Fact]
    public void EnsureCuentaActiva_Throws_WhenCuentaInactive()
    {
        var cuenta = new PlanCuenta { TenantId = Guid.NewGuid(), Codigo = "101", Descripcion = "Caja", Nivel = 3, Tipo = "Activo", Activa = false };

        Assert.Throws<InvalidOperationException>(() => AccountingRules.EnsureCuentaActiva(cuenta));
    }
}
