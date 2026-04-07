using Domain.Common;

namespace Domain.Sire;

public sealed class SirePropuesta : TenantEntity
{
    public required string Periodo { get; set; }
    public decimal ImporteTotal { get; set; }
    public SireEstadoPropuesta Estado { get; set; } = SireEstadoPropuesta.Pendiente;
    public DateTime GeneratedAtUtc { get; set; }
    public string Source { get; set; } = "bootstrap";
    public List<SireContabilizacionResultado> Resultados { get; } = new();
}
