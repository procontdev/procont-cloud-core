using Application.Common.Interfaces;
using Application.Sire;
using Domain.Sire;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class EfSireProposalRepository(ProcontDbContext dbContext, ITenantContext tenantContext) : ISireProposalRepository
{
    public async Task<IReadOnlyList<SirePropuestaResponse>> ListAsync(CancellationToken cancellationToken = default)
        => await dbContext.SirePropuestas
            .AsNoTracking()
            .Where(x => x.TenantId == tenantContext.TenantId)
            .OrderByDescending(x => x.GeneratedAtUtc)
            .Select(x => new SirePropuestaResponse(x.Id, x.Periodo, x.Estado.ToString(), x.ImporteTotal, x.GeneratedAtUtc))
            .ToListAsync(cancellationToken);

    public async Task<SirePropuestaResponse?> GetByIdAsync(Guid propuestaId, CancellationToken cancellationToken = default)
        => await dbContext.SirePropuestas
            .AsNoTracking()
            .Where(x => x.TenantId == tenantContext.TenantId && x.Id == propuestaId)
            .Select(x => new SirePropuestaResponse(x.Id, x.Periodo, x.Estado.ToString(), x.ImporteTotal, x.GeneratedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddResultAsync(Guid propuestaId, SireContabilizacionResultDraft draft, CancellationToken cancellationToken = default)
    {
        var propuesta = await dbContext.SirePropuestas
            .FirstOrDefaultAsync(x => x.Id == propuestaId && x.TenantId == tenantContext.TenantId, cancellationToken)
            ?? throw new InvalidOperationException("Propuesta SIRE no encontrada para el tenant actual.");

        propuesta.Estado = draft.Estado switch
        {
            "Aceptado" => SireEstadoPropuesta.Contabilizada,
            "Observada" => SireEstadoPropuesta.Observada,
            _ => SireEstadoPropuesta.Error
        };
        propuesta.UpdatedAt = draft.ProcessedAtUtc;

        dbContext.SireContabilizacionResultados.Add(new SireContabilizacionResultado
        {
            TenantId = tenantContext.TenantId,
            PropuestaId = propuestaId,
            AsientoId = draft.AsientoId,
            Estado = draft.Estado,
            Mensaje = draft.Mensaje,
            Observacion = draft.Observacion,
            ProcessedAtUtc = draft.ProcessedAtUtc,
            CreatedAt = draft.ProcessedAtUtc,
            UpdatedAt = draft.ProcessedAtUtc,
            CreatedBy = tenantContext.UserId
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
