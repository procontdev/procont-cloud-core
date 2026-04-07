using Application.Accounting;
using Application.Auth;
using Application.Common.Exceptions;
using Application.Sire;
using Application.Tenants;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Observability;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "PROCONT_");

builder.Services.Configure<TenantResolutionOptions>(builder.Configuration.GetSection("TenantResolution"));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Procont Cloud Core API",
        Version = "v1",
        Description = "Sprint 1 core multi-tenant + IAM base + contable habilitador"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

var securityOptions = app.Services.GetRequiredService<IOptions<SecurityOptions>>().Value;

if (!app.Environment.IsEnvironment("Testing"))
{
    await app.Services.InitializeInfrastructureAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<RequestContextEnrichmentMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(handlerApp =>
{
    handlerApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message ?? "Error inesperado."
        });
    });
});

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "procont-cloud-core-api",
    utc = DateTime.UtcNow
}))
.WithTags("Health")
.WithOpenApi();

var api = app.MapGroup("/api/v1");

api.MapPost("/auth/login", async (LoginRequest request, AuthService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.LoginAsync(request, cancellationToken)))
    .WithTags("Auth")
    .WithName("Login")
    .WithOpenApi();

api.MapGet("/tenants/{id:guid}", [Authorize(Roles = "Admin")] async (Guid id, TenantService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetByIdAsync(id, cancellationToken)))
    .WithTags("Platform")
    .WithName("GetTenant")
    .WithOpenApi();

api.MapGet("/companies", [Authorize(Policy = "Permission:platform.companies.manage")] async (CompanyService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListAsync(cancellationToken)))
    .WithTags("Platform")
    .WithName("ListCompanies")
    .WithOpenApi();

api.MapPost("/companies", [Authorize(Policy = "Permission:platform.companies.manage")] async (CreateCompanyRequest request, CompanyService service, CancellationToken cancellationToken) =>
    Results.Created($"/api/v1/companies", await service.CreateAsync(request, cancellationToken)))
    .WithTags("Platform")
    .WithName("CreateCompany")
    .WithOpenApi();

api.MapGet("/plan-cuentas", [Authorize(Policy = "Permission:accounting.manage")] async (PlanCuentaService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListAsync(cancellationToken)))
    .WithTags("Accounting")
    .WithName("ListPlanCuentas")
    .WithOpenApi();

api.MapPost("/plan-cuentas", [Authorize(Policy = "Permission:accounting.manage")] async (CreatePlanCuentaRequest request, PlanCuentaService service, CancellationToken cancellationToken) =>
    Results.Created("/api/v1/plan-cuentas", await service.CreateAsync(request, cancellationToken)))
    .WithTags("Accounting")
    .WithName("CreatePlanCuenta")
    .WithOpenApi();

api.MapGet("/periodos", [Authorize(Policy = "Permission:accounting.manage")] async (PeriodoContableService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListAsync(cancellationToken)))
    .WithTags("Accounting")
    .WithName("ListPeriodos")
    .WithOpenApi();

api.MapPost("/periodos", [Authorize(Policy = "Permission:accounting.manage")] async (CreatePeriodoContableRequest request, PeriodoContableService service, CancellationToken cancellationToken) =>
    Results.Created("/api/v1/periodos", await service.CreateAsync(request, cancellationToken)))
    .WithTags("Accounting")
    .WithName("CreatePeriodo")
    .WithOpenApi();

api.MapPost("/asientos", [Authorize(Policy = "Permission:accounting.manage")] async (CreateAsientoRequest request, AsientoService service, CancellationToken cancellationToken) =>
    Results.Created("/api/v1/asientos", await service.CreateAsync(request, cancellationToken)))
    .WithTags("Accounting")
    .WithName("CreateAsiento")
    .WithOpenApi();

api.MapGet("/asientos/{id:guid}", [Authorize(Policy = "Permission:accounting.manage")] async (Guid id, AsientoService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetByIdAsync(id, cancellationToken)))
    .WithTags("Accounting")
    .WithName("GetAsiento")
    .WithOpenApi();

api.MapGet("/sire/propuestas", [Authorize(Policy = "Permission:sire.manage")] async (SireService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListPropuestasAsync(cancellationToken)))
    .WithTags("SIRE")
    .WithName("ListSirePropuestas")
    .WithOpenApi();

api.MapPost("/sire/contabilizar", [Authorize(Policy = "Permission:sire.manage")] async (ContabilizarSireRequest request, SireService service, CancellationToken cancellationToken) =>
    Results.Accepted("/api/v1/sire/propuestas", await service.ContabilizarAsync(request, cancellationToken)))
    .WithTags("SIRE")
    .WithName("ContabilizarSire")
    .WithOpenApi();

api.MapGet("/observability/context", [Authorize] (HttpContext context) => Results.Ok(new
{
    correlationId = context.TraceIdentifier,
    tenantCode = context.Items.TryGetValue(HttpTenantContext.TenantCodeKey, out var tenantCode) ? tenantCode : null,
    tenantId = context.Items.TryGetValue(HttpTenantContext.TenantIdKey, out var tenantId) ? tenantId : null,
    secretProvider = securityOptions.SecretProvider,
    secretReference = securityOptions.SecretReference
}))
    .WithTags("Observability")
    .WithName("GetObservabilityContext")
    .WithOpenApi();

app.Run();

public partial class Program;
