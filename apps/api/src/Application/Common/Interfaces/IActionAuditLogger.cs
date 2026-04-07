namespace Application.Common.Interfaces;

public interface IActionAuditLogger
{
    Task LogAsync(string action, string resourceType, string resourceId, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
}
