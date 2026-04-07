using Domain.Common;

namespace Domain.Sire;

public sealed class SireContabilizacionResultado : TenantEntity, IHasTenantReference
{
    public required Guid PropuestaId { get; init; }
    public Guid? AsientoId { get; set; }
    public required string Estado { get; set; }
    public required string Mensaje { get; set; }
    public string? Observacion { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
}
