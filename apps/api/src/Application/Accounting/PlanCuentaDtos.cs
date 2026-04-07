namespace Application.Accounting;

public sealed record PlanCuentaResponse(Guid Id, Guid TenantId, string Codigo, string Descripcion, int Nivel, string Tipo, bool Activa, DateTime CreatedAt, Guid? CreatedBy);

public sealed record CreatePlanCuentaRequest(string Codigo, string Descripcion, int Nivel, string Tipo, bool Activa);
