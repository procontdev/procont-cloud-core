using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public sealed class ProcontDbContextFactory : IDesignTimeDbContextFactory<ProcontDbContext>
{
    public ProcontDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProcontDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("PROCONT_CONNECTIONSTRINGS__POSTGRES")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Port=5432;Database=procont_core;Username=procont_user;Password=procont_local_password";

        optionsBuilder.UseNpgsql(connectionString);
        return new ProcontDbContext(optionsBuilder.Options);
    }
}
