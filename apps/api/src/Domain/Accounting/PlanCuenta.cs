using Domain.Common;

namespace Domain.Accounting;

public sealed class PlanCuenta : TenantEntity
{
    public required string Codigo { get; init; }
    public required string Descripcion { get; set; }
    public required int Nivel { get; init; }
    public required string Tipo { get; init; }
    public bool Activa { get; set; } = true;
}
