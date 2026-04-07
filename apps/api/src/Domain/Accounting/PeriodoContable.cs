using Domain.Common;

namespace Domain.Accounting;

public sealed class PeriodoContable : TenantEntity
{
    public required int Anio { get; init; }
    public required int Mes { get; init; }
    public PeriodoEstado Estado { get; set; } = PeriodoEstado.Abierto;
}
