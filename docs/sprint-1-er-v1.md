# ER v1 - Sprint 1

```text
Tenant 1---* Company
Tenant 1---* User
Tenant 1---* Role
Role *---* Permission
User *---* Role
Tenant 1---* PlanCuenta
Tenant 1---* PeriodoContable
Tenant 1---* Asiento
Asiento 1---* AsientoDetalle
PlanCuenta 1---* AsientoDetalle
Tenant 1---* ActionAuditLog
```

## Notas

- Todas las entidades de negocio persisten `tenant_id`.
- `created_at`, `updated_at` y `created_by` forman la auditoria transversal minima.
- La implementacion actual usa persistencia en memoria como placeholder de Sprint 1; las migraciones SQL quedan pendientes para el salto a PostgreSQL con RLS.
