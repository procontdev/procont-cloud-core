namespace Infrastructure.Persistence.Rls;

public static class TenantRlsSql
{
    public static readonly string[] Scripts =
    [
        "ALTER TABLE companies ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE users ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE roles ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE plan_cuentas ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE periodos_contables ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE asientos ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE asiento_detalles ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE action_audit_logs ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE sire_propuestas ENABLE ROW LEVEL SECURITY;",
        "ALTER TABLE sire_contabilizacion_resultados ENABLE ROW LEVEL SECURITY;",
        CreatePolicy("companies"),
        CreatePolicy("users"),
        CreatePolicy("roles"),
        CreatePolicy("plan_cuentas"),
        CreatePolicy("periodos_contables"),
        CreatePolicy("asientos"),
        CreatePolicy("asiento_detalles"),
        CreatePolicy("action_audit_logs"),
        CreatePolicy("sire_propuestas"),
        CreatePolicy("sire_contabilizacion_resultados")
    ];

    private static string CreatePolicy(string tableName)
        => $"DROP POLICY IF EXISTS {tableName}_tenant_isolation ON {tableName}; CREATE POLICY {tableName}_tenant_isolation ON {tableName} USING (tenant_id = NULLIF(current_setting('{Persistence.ProcontDbContext.TenantSettingName}', true), '')::uuid) WITH CHECK (tenant_id = NULLIF(current_setting('{Persistence.ProcontDbContext.TenantSettingName}', true), '')::uuid);";
}
