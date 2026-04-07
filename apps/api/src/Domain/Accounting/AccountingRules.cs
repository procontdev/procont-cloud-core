namespace Domain.Accounting;

public static class AccountingRules
{
    public static void EnsureBalanced(IEnumerable<AsientoDetalle> detalles)
    {
        var materialized = detalles.ToList();
        if (materialized.Count == 0)
        {
            throw new InvalidOperationException("El asiento debe tener al menos un detalle.");
        }

        var totalDebe = materialized.Sum(x => x.Debe);
        var totalHaber = materialized.Sum(x => x.Haber);

        if (totalDebe != totalHaber)
        {
            throw new InvalidOperationException("El asiento debe estar balanceado.");
        }
    }

    public static void EnsurePeriodoAbierto(PeriodoContable periodo)
    {
        if (periodo.Estado == PeriodoEstado.Cerrado)
        {
            throw new InvalidOperationException("No se permite registrar asientos en periodos cerrados.");
        }
    }

    public static void EnsureCuentaActiva(PlanCuenta cuenta)
    {
        if (!cuenta.Activa)
        {
            throw new InvalidOperationException("La cuenta contable debe estar activa para ser utilizada.");
        }
    }
}
