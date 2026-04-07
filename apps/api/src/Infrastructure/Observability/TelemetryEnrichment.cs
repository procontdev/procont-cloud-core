using System.Diagnostics;

namespace Infrastructure.Observability;

public static class TelemetryEnrichment
{
    public static void SetRequestContext(Activity? activity, string? requestId, string? tenantId)
    {
        activity?.SetTag("procont.request_id", requestId);
        activity?.SetTag("procont.tenant_id", tenantId);
    }
}
