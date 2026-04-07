using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Accounting;

namespace Application.Accounting;

public sealed class AsientoService(
    IRepository<Asiento> asientoRepository,
    IRepository<PeriodoContable> periodoRepository,
    IRepository<PlanCuenta> cuentaRepository,
    ITenantContext tenantContext,
    IActionAuditLogger actionAuditLogger)
{
    public async Task<AsientoResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asiento = await asientoRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Asiento no encontrado.");
        return Map(asiento);
    }

    public async Task<AsientoResponse> CreateAsync(CreateAsientoRequest request, CancellationToken cancellationToken = default)
    {
        var periodo = await periodoRepository.GetByIdAsync(request.PeriodoId, cancellationToken) ?? throw new ValidationException("Periodo contable no existe.");
        AccountingRules.EnsurePeriodoAbierto(periodo);

        var detalles = new List<AsientoDetalle>();
        foreach (var item in request.Detalles)
        {
            var cuenta = await cuentaRepository.GetByIdAsync(item.CuentaId, cancellationToken) ?? throw new ValidationException("Cuenta contable no existe.");
            AccountingRules.EnsureCuentaActiva(cuenta);

            detalles.Add(new AsientoDetalle
            {
                TenantId = tenantContext.TenantId,
                AsientoId = Guid.Empty,
                CuentaId = item.CuentaId,
                Debe = item.Debe,
                Haber = item.Haber,
                CentroCosto = item.CentroCosto,
                CreatedBy = tenantContext.UserId
            });
        }

        AccountingRules.EnsureBalanced(detalles);

        var asiento = new Asiento
        {
            TenantId = tenantContext.TenantId,
            PeriodoId = request.PeriodoId,
            Fecha = request.Fecha,
            Glosa = request.Glosa,
            Estado = AsientoEstado.Registrado,
            CreatedBy = tenantContext.UserId
        };

        foreach (var detalle in detalles)
        {
            asiento.Detalles.Add(new AsientoDetalle
            {
                TenantId = tenantContext.TenantId,
                AsientoId = asiento.Id,
                CuentaId = detalle.CuentaId,
                Debe = detalle.Debe,
                Haber = detalle.Haber,
                CentroCosto = detalle.CentroCosto,
                CreatedBy = detalle.CreatedBy
            });
        }

        await asientoRepository.AddAsync(asiento, cancellationToken);
        await actionAuditLogger.LogAsync("asiento.create", nameof(Asiento), asiento.Id.ToString(), new Dictionary<string, string>
        {
            ["detalles"] = asiento.Detalles.Count.ToString()
        }, cancellationToken);

        return Map(asiento);
    }

    private static AsientoResponse Map(Asiento asiento) => new(
        asiento.Id,
        asiento.TenantId,
        asiento.PeriodoId,
        asiento.Fecha,
        asiento.Glosa,
        asiento.Estado,
        asiento.Detalles.Select(x => new AsientoDetalleResponse(x.Id, x.CuentaId, x.Debe, x.Haber, x.CentroCosto)).ToList(),
        asiento.CreatedAt,
        asiento.CreatedBy);
}
