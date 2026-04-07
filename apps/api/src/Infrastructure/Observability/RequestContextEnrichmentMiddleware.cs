using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Infrastructure.Observability;

public sealed class RequestContextEnrichmentMiddleware(RequestDelegate next, ILogger<RequestContextEnrichmentMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.TraceIdentifier;
        var tenantCode = context.Items.TryGetValue(HttpTenantContext.TenantCodeKey, out var code) ? code?.ToString() : "unresolved";
        var tenantId = context.Items.TryGetValue(HttpTenantContext.TenantIdKey, out var tenant) ? tenant?.ToString() : "unresolved";

        Activity.Current?.SetTag("procont.correlation_id", correlationId);
        Activity.Current?.SetTag("procont.tenant_code", tenantCode);
        Activity.Current?.SetTag("procont.tenant_id", tenantId);

        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["TenantCode"] = tenantCode,
            ["TenantId"] = tenantId
        }))
        {
            await next(context);
        }
    }
}
