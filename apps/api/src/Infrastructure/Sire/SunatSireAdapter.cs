using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application.Sire;
using Infrastructure.Configuration;
using Infrastructure.Observability;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Sire;

public sealed class SunatSireAdapter(
    HttpClient httpClient,
    IOptions<SunatSireOptions> options,
    IOptions<SecretManagementOptions> secretManagementOptions,
    ConfigurationSecretResolver secretResolver,
    IHttpContextAccessor httpContextAccessor,
    MetricsSnapshotStore metricsSnapshotStore,
    ILogger<SunatSireAdapter> logger) : ISireAdapter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly SunatSireOptions sunatOptions = options.Value;
    private readonly SecretManagementOptions secretOptions = secretManagementOptions.Value;

    public async Task<IReadOnlyList<SirePropuestaResponse>> ListPropuestasAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (sunatOptions.UseStubResponses)
        {
            logger.LogInformation("sunat_sire_stub_list tenantId={TenantId}", tenantId);
            return [];
        }

        using var activity = Telemetry.ActivitySource.StartActivity("sunat.sire.list", ActivityKind.Client);
        EnrichActivity(activity, tenantId);

        var token = await AcquireAccessTokenAsync(cancellationToken);
        var endpoint = sunatOptions.PropuestasPath;
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var response = await SendWithRetryAsync(request, tenantId, "list-propuestas", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<SunatSirePropuestasEnvelope>(JsonOptions, cancellationToken)
            ?? new SunatSirePropuestasEnvelope([]);

        return payload.Items
            .Select(item => new SirePropuestaResponse(
                item.Id,
                item.Periodo,
                item.Estado,
                item.ImporteTotal,
                item.GeneratedAtUtc))
            .ToArray();
    }

    public async Task<ContabilizarSireResponse> ContabilizarAsync(Guid tenantId, ContabilizarSireRequest request, CancellationToken cancellationToken = default)
    {
        if (sunatOptions.UseStubResponses)
        {
            logger.LogInformation("sunat_sire_stub_contabilizar tenantId={TenantId} propuestaId={PropuestaId}", tenantId, request.PropuestaId);

            return new ContabilizarSireResponse(
                request.PropuestaId,
                "Aceptado",
                "Contabilizacion simulada por configuracion local del adaptador SUNAT.",
                DateTime.UtcNow);
        }

        using var activity = Telemetry.ActivitySource.StartActivity("sunat.sire.contabilizar", ActivityKind.Client);
        EnrichActivity(activity, tenantId);
        activity?.SetTag("procont.propuesta_id", request.PropuestaId);

        var token = await AcquireAccessTokenAsync(cancellationToken);
        var endpoint = sunatOptions.ContabilizarPath.Replace("{propuestaId}", request.PropuestaId.ToString(), StringComparison.OrdinalIgnoreCase);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(new SunatContabilizarPayload(request.Observacion), options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var response = await SendWithRetryAsync(httpRequest, tenantId, "contabilizar", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<SunatContabilizarResponse>(JsonOptions, cancellationToken)
            ?? throw new SunatSireException("SUNAT respondio sin payload de contabilizacion.", (int)response.StatusCode);

        return new ContabilizarSireResponse(request.PropuestaId, payload.Estado, payload.Mensaje, payload.ProcessedAtUtc);
    }

    private async Task<SunatAuthResponse> AcquireAccessTokenAsync(CancellationToken cancellationToken)
    {
        var clientId = await secretResolver.ResolveRequiredSecretAsync("SunatSire:ClientId", "sunat-client-id", cancellationToken);
        var username = await secretResolver.ResolveOptionalSecretAsync("SunatSire:Username", secretOptions.SunatUsernameSecretName, cancellationToken);
        var password = await secretResolver.ResolveOptionalSecretAsync("SunatSire:Password", secretOptions.SunatPasswordSecretName, cancellationToken);
        var clientSecret = await secretResolver.ResolveRequiredSecretAsync("SunatSire:ClientSecret", secretOptions.SunatClientSecretName ?? "sunat-client-secret", cancellationToken);

        var body = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["scope"] = sunatOptions.Scope
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            body["username"] = username;
        }

        if (!string.IsNullOrWhiteSpace(password))
        {
            body["password"] = password;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, sunatOptions.AuthPath)
        {
            Content = new FormUrlEncodedContent(body)
        };

        var response = await SendWithRetryAsync(request, null, "auth", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<SunatAuthResponse>(JsonOptions, cancellationToken)
            ?? throw new SunatSireException("SUNAT no devolvio access token.", (int)response.StatusCode);

        return payload;
    }

    private async Task<HttpResponseMessage> SendWithRetryAsync(HttpRequestMessage request, Guid? tenantId, string operation, CancellationToken cancellationToken)
    {
        var requestId = httpContextAccessor.HttpContext?.TraceIdentifier ?? Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        var tenantTag = tenantId?.ToString() ?? httpContextAccessor.HttpContext?.Items["TenantId"]?.ToString() ?? "system";

        for (var attempt = 1; attempt <= sunatOptions.RetryCount + 1; attempt++)
        {
            using var requestClone = await CloneRequestAsync(request, cancellationToken);
            var started = Stopwatch.GetTimestamp();

            requestClone.Headers.TryAddWithoutValidation("X-Request-Id", requestId);
            requestClone.Headers.TryAddWithoutValidation("X-Tenant-Id", tenantTag);

            try
            {
                Telemetry.SunatRequests.Add(1, new KeyValuePair<string, object?>("operation", operation));
                metricsSnapshotStore.Increment("procont_sunat_requests_total");

                logger.LogInformation(
                    "sunat_request requestId={RequestId} tenantId={TenantId} operation={Operation} method={Method} path={Path} attempt={Attempt}",
                    requestId,
                    tenantTag,
                    operation,
                    requestClone.Method,
                    requestClone.RequestUri?.PathAndQuery,
                    attempt);

                var response = await httpClient.SendAsync(requestClone, cancellationToken);
                var elapsedMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                Telemetry.SunatLatencyMs.Record(elapsedMs, new KeyValuePair<string, object?>("operation", operation));
                metricsSnapshotStore.Set("procont_sunat_last_latency_ms", elapsedMs);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation(
                        "sunat_response requestId={RequestId} tenantId={TenantId} operation={Operation} statusCode={StatusCode} attempt={Attempt} elapsedMs={ElapsedMs}",
                        requestId,
                        tenantTag,
                        operation,
                        (int)response.StatusCode,
                        attempt,
                        elapsedMs);

                    return response;
                }

                var sanitizedBody = await SanitizeBodyAsync(response.Content, cancellationToken);
                logger.LogWarning(
                    "sunat_response_error requestId={RequestId} tenantId={TenantId} operation={Operation} statusCode={StatusCode} attempt={Attempt} response={Response}",
                    requestId,
                    tenantTag,
                    operation,
                    (int)response.StatusCode,
                    attempt,
                    sanitizedBody);

                if (attempt <= sunatOptions.RetryCount && IsTransient(response.StatusCode))
                {
                    Telemetry.SunatRetries.Add(1, new KeyValuePair<string, object?>("operation", operation));
                    metricsSnapshotStore.Increment("procont_sunat_retries_total");
                    await Task.Delay(ComputeDelay(attempt), cancellationToken);
                    continue;
                }

                Telemetry.SunatFailures.Add(1, new KeyValuePair<string, object?>("operation", operation));
                metricsSnapshotStore.Increment("procont_sunat_failures_total");
                throw new SunatSireException($"SUNAT devolvio error para '{operation}'.", (int)response.StatusCode, sanitizedBody);
            }
            catch (Exception exception) when (attempt <= sunatOptions.RetryCount && IsTransient(exception))
            {
                Telemetry.SunatRetries.Add(1, new KeyValuePair<string, object?>("operation", operation));
                metricsSnapshotStore.Increment("procont_sunat_retries_total");
                logger.LogWarning(exception, "sunat_retry requestId={RequestId} tenantId={TenantId} operation={Operation} attempt={Attempt}", requestId, tenantTag, operation, attempt);
                await Task.Delay(ComputeDelay(attempt), cancellationToken);
            }
            catch (Exception exception) when (exception is not SunatSireException)
            {
                Telemetry.SunatFailures.Add(1, new KeyValuePair<string, object?>("operation", operation));
                metricsSnapshotStore.Increment("procont_sunat_failures_total");
                throw new SunatSireException($"Fallo la operacion SUNAT '{operation}'.", null, null, exception);
            }
        }

        throw new SunatSireException($"No se pudo completar la operacion SUNAT '{operation}' tras reintentos.");
    }

    private void EnrichActivity(Activity? activity, Guid tenantId)
    {
        var requestId = httpContextAccessor.HttpContext?.TraceIdentifier;
        TelemetryEnrichment.SetRequestContext(activity, requestId, tenantId.ToString());
        activity?.SetTag("procont.integration", "sunat-sire");
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content is not null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            clone.Content = new ByteArrayContent(bytes);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }

    private static bool IsTransient(HttpStatusCode statusCode)
        => statusCode == HttpStatusCode.RequestTimeout
            || statusCode == HttpStatusCode.TooManyRequests
            || (int)statusCode >= 500;

    private static bool IsTransient(Exception exception)
        => exception is HttpRequestException or TaskCanceledException;

    private TimeSpan ComputeDelay(int attempt)
        => TimeSpan.FromMilliseconds(sunatOptions.RetryBaseDelayMs * Math.Pow(2, attempt - 1));

    private static async Task<string> SanitizeBodyAsync(HttpContent content, CancellationToken cancellationToken)
    {
        var body = await content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        return body
            .Replace("access_token", "redacted_token", StringComparison.OrdinalIgnoreCase)
            .Replace("client_secret", "redacted_secret", StringComparison.OrdinalIgnoreCase)
            .Replace("password", "redacted_password", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record SunatAuthResponse(string AccessToken, string TokenType, int ExpiresIn);
    private sealed record SunatSirePropuestasEnvelope(IReadOnlyList<SunatSirePropuestaItem> Items);
    private sealed record SunatSirePropuestaItem(Guid Id, string Periodo, string Estado, decimal ImporteTotal, DateTime GeneratedAtUtc);
    private sealed record SunatContabilizarPayload(string Observacion);
    private sealed record SunatContabilizarResponse(string Estado, string Mensaje, DateTime ProcessedAtUtc);
}
