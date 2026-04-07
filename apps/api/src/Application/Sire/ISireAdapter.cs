namespace Application.Sire;

public interface ISireAdapter
{
    Task<IReadOnlyList<SirePropuestaResponse>> ListPropuestasAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<ContabilizarSireResponse> ContabilizarAsync(Guid tenantId, ContabilizarSireRequest request, CancellationToken cancellationToken = default);
}
