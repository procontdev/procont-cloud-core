using Domain.Common;

namespace Domain.Accounting;

public sealed class Asiento : TenantEntity
{
    public required Guid PeriodoId { get; init; }
    public required DateOnly Fecha { get; init; }
    public required string Glosa { get; set; }
    public AsientoEstado Estado { get; set; } = AsientoEstado.Registrado;
    public List<AsientoDetalle> Detalles { get; } = new();
}
