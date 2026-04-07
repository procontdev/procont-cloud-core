using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Tenancy;

public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public const string TenantIdKey = "tenant-id";
    public const string TenantCodeKey = "tenant-code";

    private HttpContext HttpContext => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No hay HttpContext activo.");

    public Guid TenantId => HttpContext.Items.TryGetValue(TenantIdKey, out var value) && value is Guid tenantId
        ? tenantId
        : throw new InvalidOperationException("Tenant no resuelto.");

    public string TenantCode => HttpContext.Items.TryGetValue(TenantCodeKey, out var value) && value is string tenantCode
        ? tenantCode
        : throw new InvalidOperationException("Tenant no resuelto.");

    public Guid? UserId => Guid.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;

    public string? UserEmail => HttpContext.User.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
}
