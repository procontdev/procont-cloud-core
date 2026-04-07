using Domain.Accounting;

namespace Application.Accounting;

public sealed record AsientoDetalleRequest(Guid CuentaId, decimal Debe, decimal Haber, string? CentroCosto);

public sealed record CreateAsientoRequest(Guid PeriodoId, DateOnly Fecha, string Glosa, IReadOnlyList<AsientoDetalleRequest> Detalles);

public sealed record AsientoDetalleResponse(Guid Id, Guid CuentaId, decimal Debe, decimal Haber, string? CentroCosto);

public sealed record AsientoResponse(Guid Id, Guid TenantId, Guid PeriodoId, DateOnly Fecha, string Glosa, AsientoEstado Estado, IReadOnlyList<AsientoDetalleResponse> Detalles, DateTime CreatedAt, Guid? CreatedBy);
