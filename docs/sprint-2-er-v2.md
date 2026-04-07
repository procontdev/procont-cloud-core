# ER v2 - Sprint 2

```text
Tenant 1---* Company
Tenant 1---* User
Tenant 1---* Role
User *---* Role
Role *---* Permission
Tenant 1---* PlanCuenta
Tenant 1---* PeriodoContable
Tenant 1---* Asiento
PeriodoContable 1---* Asiento
Asiento 1---* AsientoDetalle
PlanCuenta 1---* AsientoDetalle
Tenant 1---* ActionAuditLog
```

## Cambios principales

- Persistencia migrada a PostgreSQL con EF Core y migraciones versionadas.
- Indices por `tenant_id` en entidades multi-tenant y unicos compuestos en `companies`, `plan_cuentas`, `periodos_contables`, `roles` y `users`.
- Llaves foraneas explicitas entre `tenant -> company/user/role/plan_cuenta/periodo/asiento`, `periodo -> asiento`, `asiento -> asiento_detalle`, `plan_cuenta -> asiento_detalle`.
- `action_audit_logs.metadata_json` persiste metadatos en `jsonb`.
- Base IAM lista para policies por permiso: `permissions`, `roles`, `role_permissions`, `user_roles`.

## Notas

- El modulo SIRE se mantiene desacoplado a nivel de contratos y adaptador mock; no agrega tablas propias en Sprint 2.
- La semilla inicial crea tenant demo, usuario admin, permisos base y catalogo contable minimo para pruebas HTTP e integracion local.
