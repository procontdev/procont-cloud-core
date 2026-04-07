using Application.Common.Interfaces;
using Domain.Accounting;

namespace Application.Accounting;

public sealed class PeriodoContableService(
    IRepository<PeriodoContable> repository,
    ITenantContext tenantContext,
    IActionAuditLogger actionAuditLogger)
{
    public async Task<IReadOnlyList<PeriodoContableResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var periodos = await repository.ListAsync(cancellationToken);
        return periodos.Select(Map).ToList();
    }

    public async Task<PeriodoContableResponse> CreateAsync(CreatePeriodoContableRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new PeriodoContable
        {
            TenantId = tenantContext.TenantId,
            Anio = request.Anio,
            Mes = request.Mes,
            Estado = request.Estado,
            CreatedBy = tenantContext.UserId
        };

        await repository.AddAsync(entity, cancellationToken);
        await actionAuditLogger.LogAsync("periodo.create", nameof(PeriodoContable), entity.Id.ToString(), cancellationToken: cancellationToken);
        return Map(entity);
    }

    private static PeriodoContableResponse Map(PeriodoContable entity) => new(entity.Id, entity.TenantId, entity.Anio, entity.Mes, entity.Estado, entity.CreatedAt, entity.CreatedBy);
}
