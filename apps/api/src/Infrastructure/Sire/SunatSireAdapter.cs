using Application.Sire;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sire;

public sealed class SunatSireAdapter(ILogger<SunatSireAdapter> logger) : ISireAdapter
{
    public Task<IReadOnlyList<SirePropuestaResponse>> ListPropuestasAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SirePropuestaResponse>>([]);

    public Task<ContabilizarSireResponse> ContabilizarAsync(Guid tenantId, ContabilizarSireRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("sire_adapter_stub tenantId={TenantId} propuestaId={PropuestaId}", tenantId, request.PropuestaId);

        return Task.FromResult(new ContabilizarSireResponse(
            request.PropuestaId,
            "Aceptado",
            "Contabilizacion persistida localmente. Adaptador SUNAT desacoplado listo para integracion posterior.",
            DateTime.UtcNow));
    }
}
