namespace Application.Sire;

public sealed record SirePropuestaResponse(Guid Id, string Periodo, string Estado, decimal ImporteTotal, DateTime GeneratedAtUtc);

public sealed record ContabilizarSireRequest(Guid PropuestaId, string Observacion);

public sealed record ContabilizarSireResponse(Guid PropuestaId, string Estado, string Mensaje, DateTime ProcessedAtUtc);
