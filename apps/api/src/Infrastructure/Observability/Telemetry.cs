using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Infrastructure.Observability;

public static class Telemetry
{
    public const string ServiceName = "procont-cloud-core-api";

    public static readonly ActivitySource ActivitySource = new(ServiceName);
    public static readonly Meter Meter = new(ServiceName);
    public static readonly Counter<long> SunatRequests = Meter.CreateCounter<long>("procont.sunat.requests");
    public static readonly Counter<long> SunatRetries = Meter.CreateCounter<long>("procont.sunat.retries");
    public static readonly Counter<long> SunatFailures = Meter.CreateCounter<long>("procont.sunat.failures");
    public static readonly Histogram<double> SunatLatencyMs = Meter.CreateHistogram<double>("procont.sunat.latency.ms");
}
