using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Tests.Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Application.Tests;

public sealed class ApiIntegrationTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task Login_ReturnsJwtToken()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Code", "demo");

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@demo.local",
            password = "Procont2026*"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.AccessToken));
    }

    [Fact]
    public async Task MissingTenantHeader_ReturnsBadRequest()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@demo.local",
            password = "Procont2026*"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsiento_WithPermission_ReturnsCreated()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Code", "demo");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@demo.local",
            password = "Procont2026*"
        });

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);

        var planCuentas = await client.GetFromJsonAsync<List<PlanCuentaPayload>>("/api/v1/plan-cuentas");
        var periodos = await client.GetFromJsonAsync<List<PeriodoPayload>>("/api/v1/periodos");

        var response = await client.PostAsJsonAsync("/api/v1/asientos", new
        {
            periodoId = periodos![0].Id,
            fecha = "2026-04-07",
            glosa = "Asiento integration test",
            detalles = new[]
            {
                new { cuentaId = planCuentas![0].Id, debe = 150.00m, haber = 0.00m, centroCosto = (string?)null },
                new { cuentaId = planCuentas![1].Id, debe = 0.00m, haber = 150.00m, centroCosto = (string?)null }
            }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SireFlow_PersistsResultInPostgres()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Code", "demo");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@demo.local",
            password = "Procont2026*"
        });

        var authPayload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authPayload!.AccessToken);

        var propuestas = await client.GetFromJsonAsync<List<SirePropuestaPayload>>("/api/v1/sire/propuestas");
        var propuesta = Assert.Single(propuestas!, x => x.Id == Guid.Parse("55555555-5555-5555-5555-555555555551"));

        var contabilizarResponse = await client.PostAsJsonAsync("/api/v1/sire/contabilizar", new
        {
            propuestaId = propuesta.Id,
            observacion = "integration-test"
        });

        Assert.Equal(HttpStatusCode.Accepted, contabilizarResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProcontDbContext>();
        var persisted = await dbContext.SireContabilizacionResultados.AsNoTracking().SingleAsync();
        Assert.Equal(propuesta.Id, persisted.PropuestaId);
        Assert.Equal("Aceptado", persisted.Estado);
    }

    [Fact]
    public async Task Rls_IsolatesTenantRows_WhenSessionTenantChanges()
    {
        await using var connection = new NpgsqlConnection(factory.ConnectionString);
        await connection.OpenAsync();

        await using (var insertCommand = connection.CreateCommand())
        {
            insertCommand.CommandText = "insert into tenants (id, code, name, status, created_at, updated_at) values (@id, @code, @name, @status, now(), now()) on conflict (id) do nothing;";
            insertCommand.Parameters.AddWithValue("id", Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            insertCommand.Parameters.AddWithValue("code", "alt");
            insertCommand.Parameters.AddWithValue("name", "Tenant Alt");
            insertCommand.Parameters.AddWithValue("status", "Active");
            await insertCommand.ExecuteNonQueryAsync();
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "select set_config('app.current_tenant', @tenantId, false); select count(*) from sire_propuestas;";
            command.Parameters.AddWithValue("tenantId", Guid.Parse("11111111-1111-1111-1111-111111111111").ToString());
            var count = (long)(await command.ExecuteScalarAsync())!;
            Assert.True(count >= 2);
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "select set_config('app.current_tenant', @tenantId, false); select count(*) from sire_propuestas;";
            command.Parameters.AddWithValue("tenantId", Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa").ToString());
            var count = (long)(await command.ExecuteScalarAsync())!;
            Assert.Equal(0, count);
        }
    }

    [Fact]
    public async Task ObservabilityContext_ReturnsCorrelationAndTenantMetadata()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Code", "demo");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@demo.local",
            password = "Procont2026*"
        });

        var authPayload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authPayload!.AccessToken);

        var response = await client.GetFromJsonAsync<ObservabilityPayload>("/api/v1/observability/context");
        Assert.False(string.IsNullOrWhiteSpace(response?.CorrelationId));
        Assert.Equal("demo", response?.TenantCode);
        Assert.Equal("integration-tests", response?.SecretProvider);
    }

    private sealed record LoginPayload(string AccessToken, string TokenType, DateTime ExpiresAtUtc, string[] Roles);
    private sealed record PlanCuentaPayload(Guid Id, Guid TenantId, string Codigo, string Descripcion, int Nivel, string Tipo, bool Activa, DateTime CreatedAt, Guid? CreatedBy);
    private sealed record PeriodoPayload(Guid Id, Guid TenantId, int Anio, int Mes, int Estado, DateTime CreatedAt, Guid? CreatedBy);
    private sealed record SirePropuestaPayload(Guid Id, string Periodo, string Estado, decimal ImporteTotal, DateTime GeneratedAtUtc);
    private sealed record ObservabilityPayload(string CorrelationId, string RequestId, string TenantCode, string TenantId, string SecretProvider, string SecretReference);
}
