namespace Domain.Common;

public sealed class ActionAuditLog : AuditableEntity
{
    public required Guid TenantId { get; init; }
    public required string UserEmail { get; init; }
    public required string Action { get; init; }
    public required string ResourceType { get; init; }
    public required string ResourceId { get; init; }
    public required string TraceId { get; init; }
    public required DateTime OccurredAt { get; init; }
    public required IReadOnlyDictionary<string, string> Metadata { get; init; }
}
