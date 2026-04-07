# Sprint 4 - Observability Report

- Structured JSON logging enabled globally.
- OpenTelemetry tracing and metrics instrumentation added for ASP.NET Core, `HttpClient` and SUNAT integration spans.
- Correlation fields `request_id` and `tenant_id` are attached to logging scopes and activity tags.
- Prometheus-style metrics snapshot endpoint exposed at `/metrics`.

## Instrumented assets

- Middleware enrichment: `apps/api/src/Infrastructure/Observability/RequestContextEnrichmentMiddleware.cs`.
- Telemetry primitives: `apps/api/src/Infrastructure/Observability/Telemetry.cs`.
- Metrics store/export: `apps/api/src/Infrastructure/Observability/MetricsSnapshotStore.cs` and `apps/api/src/Infrastructure/Observability/MetricsEndpointExtensions.cs`.
- Host wiring: `apps/api/src/Api/Program.cs`.

## Exported signals

- Logs: JSON console + OpenTelemetry log exporter pipeline.
- Metrics: `procont_sunat_requests_total`, `procont_sunat_retries_total`, `procont_sunat_failures_total`, `procont_sunat_last_latency_ms`.
- Traces: HTTP server spans, outbound HTTP spans, custom SUNAT spans.

## Operational use

- Use `request_id` for incident correlation across API logs and SUNAT outbound requests.
- Use `tenant_id` for pilot isolation and per-study monitoring.
