using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Application.Tests.Infrastructure;

public sealed class PostgresTestHarness : IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "procont_core_test",
            Username = "procont_user",
            Password = "procont_local_password"
        })
        .WithImage("postgres:16-alpine")
        .Build();

    public string ConnectionString => container.ConnectionString;

    public Task InitializeAsync() => container.StartAsync();

    public Task DisposeAsync() => container.DisposeAsync().AsTask();
}
