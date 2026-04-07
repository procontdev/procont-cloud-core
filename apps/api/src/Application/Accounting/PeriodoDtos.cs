using Domain.Accounting;

namespace Application.Accounting;

public sealed record PeriodoContableResponse(Guid Id, Guid TenantId, int Anio, int Mes, PeriodoEstado Estado, DateTime CreatedAt, Guid? CreatedBy);

public sealed record CreatePeriodoContableRequest(int Anio, int Mes, PeriodoEstado Estado);
