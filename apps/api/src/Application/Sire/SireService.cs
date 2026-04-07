using Application.Common.Interfaces;

namespace Application.Sire;

public sealed class SireService(
    ISireAdapter sireAdapter,
    ISireProposalRepository sireProposalRepository,
    ITenantContext tenantContext,
    IActionAuditLogger actionAuditLogger)
{
    public Task<IReadOnlyList<SirePropuestaResponse>> ListPropuestasAsync(CancellationToken cancellationToken = default)
        => sireProposalRepository.ListAsync(cancellationToken);

    public async Task<ContabilizarSireResponse> ContabilizarAsync(ContabilizarSireRequest request, CancellationToken cancellationToken = default)
    {
        _ = await sireProposalRepository.GetByIdAsync(request.PropuestaId, cancellationToken)
            ?? throw new InvalidOperationException("Propuesta SIRE no encontrada.");

        var response = await sireAdapter.ContabilizarAsync(tenantContext.TenantId, request, cancellationToken);

        await sireProposalRepository.AddResultAsync(
            request.PropuestaId,
            new SireContabilizacionResultDraft(null, response.Estado, response.Mensaje, request.Observacion, response.ProcessedAtUtc),
            cancellationToken);

        await actionAuditLogger.LogAsync(
            "sire.contabilizar",
            "SirePropuesta",
            request.PropuestaId.ToString(),
            new Dictionary<string, string> { ["estado"] = response.Estado },
            cancellationToken);

        return response;
    }
}
