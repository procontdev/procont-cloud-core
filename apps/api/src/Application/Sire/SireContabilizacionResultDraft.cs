namespace Application.Sire;

public sealed record SireContabilizacionResultDraft(Guid? AsientoId, string Estado, string Mensaje, string? Observacion, DateTime ProcessedAtUtc);
