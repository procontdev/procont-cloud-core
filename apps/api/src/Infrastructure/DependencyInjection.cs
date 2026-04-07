using Application.Accounting;
using Application.Auth;
using Application.Common.Interfaces;
using Application.Tenants;
using Domain.Accounting;
using Domain.Common;
using Domain.Iam;
using Domain.Tenants;
using Infrastructure.Auth;
using Infrastructure.Auditing;
using Infrastructure.Configuration;
using Infrastructure.Observability;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Rls;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Infrastructure.Security;
using Infrastructure.Sire;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<PostgresOptions>(configuration.GetSection(PostgresOptions.SectionName));
        services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.SectionName));
        services.Configure<SecretManagementOptions>(configuration.GetSection(SecretManagementOptions.SectionName));
        services.Configure<SunatSireOptions>(configuration.GetSection(SunatSireOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? Environment.GetEnvironmentVariable("PROCONT_CONNECTIONSTRINGS__POSTGRES")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres no configurado.");

        services.AddHttpContextAccessor();
        services.AddScoped<TenantSessionDbConnectionInterceptor>();
        services.AddDbContext<ProcontDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString);

            var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (httpContext is not null && httpContext.Items.ContainsKey(HttpTenantContext.TenantIdKey))
            {
                options.AddInterceptors(serviceProvider.GetRequiredService<TenantSessionDbConnectionInterceptor>());
            }
        });

        services.AddScoped<ITenantRepository, EfTenantRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IRepository<Company>, EfTenantEntityRepository<Company>>();
        services.AddScoped<IRepository<PlanCuenta>, EfTenantEntityRepository<PlanCuenta>>();
        services.AddScoped<IRepository<PeriodoContable>, EfTenantEntityRepository<PeriodoContable>>();
        services.AddScoped<IRepository<Asiento>, EfTenantEntityRepository<Asiento>>();
        services.AddScoped<IRepository<ActionAuditLog>, EfRepository<ActionAuditLog>>();
        services.AddScoped<ISireProposalRepository, EfSireProposalRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddScoped<IActionAuditLogger, ActionAuditLogger>();
        services.AddScoped<RequestContextEnrichmentMiddleware>();
        services.AddSingleton<BCryptPasswordHasher>();
        services.AddSingleton<IPasswordHasher>(sp => sp.GetRequiredService<BCryptPasswordHasher>());
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ISecretProvider, EnvironmentSecretProvider>();
        services.AddSingleton<ConfigurationSecretResolver>();
        services.AddSingleton<MetricsSnapshotStore>();
        services.AddScoped<ProcontDbSeeder>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddHttpClient<SunatSireAdapter>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<SunatSireOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });
        services.TryAddScoped<Application.Sire.ISireAdapter, SunatSireAdapter>();

        services.AddScoped<AuthService>();
        services.AddScoped<TenantService>();
        services.AddScoped<CompanyService>();
        services.AddScoped<PlanCuentaService>();
        services.AddScoped<PeriodoContableService>();
        services.AddScoped<AsientoService>();
        services.AddScoped<Application.Sire.SireService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
                if (string.IsNullOrWhiteSpace(jwtOptions.Key))
                {
                    throw new InvalidOperationException("Jwt:Key no configurado. Usa variables de entorno o user secrets.");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Permission:platform.companies.manage", policy => policy.Requirements.Add(new PermissionRequirement("platform.companies.manage")));
            options.AddPolicy("Permission:accounting.manage", policy => policy.Requirements.Add(new PermissionRequirement("accounting.manage")));
            options.AddPolicy("Permission:sire.manage", policy => policy.Requirements.Add(new PermissionRequirement("sire.manage")));
        });

        return services;
    }

    public static async Task InitializeInfrastructureAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProcontDbContext>();
        var postgresOptions = scope.ServiceProvider.GetRequiredService<IOptions<PostgresOptions>>().Value;

        if (postgresOptions.AutoMigrate)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            if (postgresOptions.EnableRls)
            {
                foreach (var script in TenantRlsSql.Scripts)
                {
                    await dbContext.Database.ExecuteSqlRawAsync(script, cancellationToken);
                }
            }
        }

        if (postgresOptions.SeedDemoData)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<ProcontDbSeeder>();
            await seeder.SeedAsync(cancellationToken);
        }
    }
}
