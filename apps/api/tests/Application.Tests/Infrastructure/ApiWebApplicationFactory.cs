using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace Application.Tests.Infrastructure;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgresTestHarness postgres = new();
    private bool initialized;

    public string ConnectionString => postgres.ConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "procont-cloud-core-test",
                ["Jwt:Audience"] = "procont-cloud-clients-test",
                ["Jwt:Key"] = "integration-tests-jwt-key-32-chars!!",
                ["ConnectionStrings:Postgres"] = postgres.ConnectionString,
                ["Postgres:AutoMigrate"] = "true",
                ["Postgres:SeedDemoData"] = "true",
                ["Postgres:EnableRls"] = "true",
                ["Security:SecretProvider"] = "integration-tests",
                ["Security:SecretReference"] = "in-memory",
                ["Security:RotateJwtKeys"] = "true",
                ["Security:JwtActiveKeyVersion"] = "test-v1",
                ["Security:AllowedJwtKeyVersions:0"] = "test-v1",
                ["SecretManagement:Environment"] = "test",
                ["SecretManagement:Backend"] = "in-memory",
                ["SecretManagement:EnforceExternalSecrets"] = "false",
                ["SecretManagement:RequireJwtKeyRotation"] = "true",
                ["SecretManagement:JwtKeySecretName"] = "jwt-signing-key",
                ["SecretManagement:JwtKeyVersion"] = "test-v1",
                ["SunatSire:UseStubResponses"] = "true",
                ["SunatSire:ClientId"] = "sunat-client-id-test",
                ["SunatSire:ClientSecret"] = "sunat-client-secret-test"
            });
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ProcontDbContext>>();
            services.RemoveAll<ProcontDbContext>();
            services.AddDbContext<ProcontDbContext>(options => options.UseNpgsql(postgres.ConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        if (initialized)
        {
            return;
        }

        await postgres.InitializeAsync();

        _ = CreateClient();
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProcontDbContext>();
        await dbContext.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<ProcontDbSeeder>();
        await seeder.SeedAsync();
        initialized = true;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await postgres.DisposeAsync();
        Dispose();
    }
}
