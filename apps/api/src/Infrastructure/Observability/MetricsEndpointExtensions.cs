using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Observability;

public static class MetricsEndpointExtensions
{
    public static IEndpointRouteBuilder MapPrometheusMetrics(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/metrics", (MetricsSnapshotStore store) => Results.Text(store.ExportPrometheus(), "text/plain"));
        return endpoints;
    }
}
