using Domain.Common;

namespace Domain.Accounting;

public sealed class AsientoDetalle : AuditableEntity, IHasTenantReference
{
    public Guid TenantId { get; init; }
    public required Guid AsientoId { get; init; }
    public required Guid CuentaId { get; init; }
    public decimal Debe { get; init; }
    public decimal Haber { get; init; }
    public string? CentroCosto { get; init; }
}
