using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Auditing;

public sealed class ActionAuditLogger(
    IRepository<ActionAuditLog> repository,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor)
    : IActionAuditLogger
{
    public Task LogAsync(string action, string resourceType, string resourceId, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        var log = new ActionAuditLog
        {
            TenantId = tenantContext.TenantId,
            UserEmail = tenantContext.UserEmail ?? "anonymous",
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            TraceId = httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N"),
            OccurredAt = DateTime.UtcNow,
            Metadata = metadata ?? new Dictionary<string, string>(),
            CreatedBy = tenantContext.UserId
        };

        return repository.AddAsync(log, cancellationToken);
    }
}
