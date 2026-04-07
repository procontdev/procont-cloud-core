using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using System.Data.Common;

namespace Infrastructure.Tenancy;

public sealed class TenantSessionDbConnectionInterceptor(ITenantContext tenantContext) : DbConnectionInterceptor
{
    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await SetTenantAsync(connection, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetTenant(connection);
        base.ConnectionOpened(connection, eventData);
    }

    private void SetTenant(DbConnection connection)
    {
        if (connection is not NpgsqlConnection npgsqlConnection)
        {
            return;
        }

        using var command = npgsqlConnection.CreateCommand();
        command.CommandText = $"select set_config('{Persistence.ProcontDbContext.TenantSettingName}', @tenantId, false);";
        command.Parameters.AddWithValue("tenantId", tenantContext.TenantId.ToString());
        command.ExecuteNonQuery();
    }

    private async Task SetTenantAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        if (connection is not NpgsqlConnection npgsqlConnection)
        {
            return;
        }

        await using var command = npgsqlConnection.CreateCommand();
        command.CommandText = $"select set_config('{Persistence.ProcontDbContext.TenantSettingName}', @tenantId, false);";
        command.Parameters.AddWithValue("tenantId", tenantContext.TenantId.ToString());
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
