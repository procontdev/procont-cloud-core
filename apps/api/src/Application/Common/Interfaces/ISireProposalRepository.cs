namespace Application.Common.Interfaces;

using Application.Sire;

public interface ISireProposalRepository
{
    Task<IReadOnlyList<SirePropuestaResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<SirePropuestaResponse?> GetByIdAsync(Guid propuestaId, CancellationToken cancellationToken = default);
    Task AddResultAsync(Guid propuestaId, SireContabilizacionResultDraft draft, CancellationToken cancellationToken = default);
}
