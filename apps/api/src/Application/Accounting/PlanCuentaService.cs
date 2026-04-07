using Application.Common.Interfaces;
using Domain.Accounting;

namespace Application.Accounting;

public sealed class PlanCuentaService(
    IRepository<PlanCuenta> repository,
    ITenantContext tenantContext,
    IActionAuditLogger actionAuditLogger)
{
    public async Task<IReadOnlyList<PlanCuentaResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var cuentas = await repository.ListAsync(cancellationToken);
        return cuentas.Select(Map).ToList();
    }

    public async Task<PlanCuentaResponse> CreateAsync(CreatePlanCuentaRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new PlanCuenta
        {
            TenantId = tenantContext.TenantId,
            Codigo = request.Codigo,
            Descripcion = request.Descripcion,
            Nivel = request.Nivel,
            Tipo = request.Tipo,
            Activa = request.Activa,
            CreatedBy = tenantContext.UserId
        };

        await repository.AddAsync(entity, cancellationToken);
        await actionAuditLogger.LogAsync("plan-cuenta.create", nameof(PlanCuenta), entity.Id.ToString(), cancellationToken: cancellationToken);
        return Map(entity);
    }

    private static PlanCuentaResponse Map(PlanCuenta entity) => new(entity.Id, entity.TenantId, entity.Codigo, entity.Descripcion, entity.Nivel, entity.Tipo, entity.Activa, entity.CreatedAt, entity.CreatedBy);
}
