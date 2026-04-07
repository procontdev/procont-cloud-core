namespace Infrastructure.Configuration;

public sealed class PostgresOptions
{
    public const string SectionName = "Postgres";

    public bool AutoMigrate { get; set; } = true;
    public bool SeedDemoData { get; set; } = true;
    public bool EnableRls { get; set; } = true;
}
