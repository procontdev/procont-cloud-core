ALTER TABLE companies ENABLE ROW LEVEL SECURITY;
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE plan_cuentas ENABLE ROW LEVEL SECURITY;
ALTER TABLE periodos_contables ENABLE ROW LEVEL SECURITY;
ALTER TABLE asientos ENABLE ROW LEVEL SECURITY;
ALTER TABLE asiento_detalles ENABLE ROW LEVEL SECURITY;
ALTER TABLE action_audit_logs ENABLE ROW LEVEL SECURITY;
ALTER TABLE sire_propuestas ENABLE ROW LEVEL SECURITY;
ALTER TABLE sire_contabilizacion_resultados ENABLE ROW LEVEL SECURITY;

DROP POLICY IF EXISTS companies_tenant_isolation ON companies;
CREATE POLICY companies_tenant_isolation ON companies
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS users_tenant_isolation ON users;
CREATE POLICY users_tenant_isolation ON users
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS roles_tenant_isolation ON roles;
CREATE POLICY roles_tenant_isolation ON roles
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS plan_cuentas_tenant_isolation ON plan_cuentas;
CREATE POLICY plan_cuentas_tenant_isolation ON plan_cuentas
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS periodos_contables_tenant_isolation ON periodos_contables;
CREATE POLICY periodos_contables_tenant_isolation ON periodos_contables
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS asientos_tenant_isolation ON asientos;
CREATE POLICY asientos_tenant_isolation ON asientos
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS asiento_detalles_tenant_isolation ON asiento_detalles;
CREATE POLICY asiento_detalles_tenant_isolation ON asiento_detalles
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS action_audit_logs_tenant_isolation ON action_audit_logs;
CREATE POLICY action_audit_logs_tenant_isolation ON action_audit_logs
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS sire_propuestas_tenant_isolation ON sire_propuestas;
CREATE POLICY sire_propuestas_tenant_isolation ON sire_propuestas
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);

DROP POLICY IF EXISTS sire_contabilizacion_resultados_tenant_isolation ON sire_contabilizacion_resultados;
CREATE POLICY sire_contabilizacion_resultados_tenant_isolation ON sire_contabilizacion_resultados
USING (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid)
WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant', true), '')::uuid);
