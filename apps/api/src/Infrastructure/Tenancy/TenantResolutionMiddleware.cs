using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Tenancy;

public sealed class TenantResolutionMiddleware(
    RequestDelegate next,
    ITenantRepository tenantRepository,
    IOptions<TenantResolutionOptions> options,
    ILogger<TenantResolutionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var tenantCode = ResolveTenantCode(context, options.Value);
        if (string.IsNullOrWhiteSpace(tenantCode))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant no especificado." });
            return;
        }

        var tenant = await tenantRepository.GetByCodeAsync(tenantCode);
        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant no encontrado." });
            return;
        }

        context.Items[HttpTenantContext.TenantIdKey] = tenant.Id;
        context.Items[HttpTenantContext.TenantCodeKey] = tenant.Code;
        logger.LogInformation("tenant_resolved code={TenantCode} tenantId={TenantId}", tenant.Code, tenant.Id);
        await next(context);
    }

    private static string? ResolveTenantCode(HttpContext context, TenantResolutionOptions options)
    {
        if (context.Request.Headers.TryGetValue(options.HeaderName, out var headerValue) && !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString().Trim().ToLowerInvariant();
        }

        if (context.Request.Query.TryGetValue(options.QueryKey, out var queryValue) && !string.IsNullOrWhiteSpace(queryValue))
        {
            return queryValue.ToString().Trim().ToLowerInvariant();
        }

        if (options.AllowSubdomainResolution && context.Request.Host.Host.Contains('.'))
        {
            return context.Request.Host.Host.Split('.')[0].Trim().ToLowerInvariant();
        }

        return null;
    }
}
