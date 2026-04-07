# Sprint 3 ER v3

## Entidades clave

- `tenants`: raiz del aislamiento logico.
- `companies`, `users`, `roles`, `plan_cuentas`, `periodos_contables`, `asientos`, `asiento_detalles`, `action_audit_logs`: tablas de negocio con `tenant_id` y RLS.
- `sire_propuestas`: propuestas persistidas por tenant para el flujo base SIRE.
- `sire_contabilizacion_resultados`: historial de contabilizacion de propuestas SIRE por tenant.

## Relaciones nuevas

- `sire_propuestas.tenant_id -> tenants.id`
- `sire_contabilizacion_resultados.tenant_id -> tenants.id`
- `sire_contabilizacion_resultados.propuesta_id -> sire_propuestas.id`
- `sire_contabilizacion_resultados.asiento_id -> asientos.id`
- `asiento_detalles.tenant_id -> tenants.id`

## Notas de diseno

- Todas las tablas de negocio con `tenant_id` tienen politicas RLS basadas en `current_setting('app.current_tenant')`.
- `asiento_detalles` incorpora `tenant_id` para evitar bypass indirecto por joins.
- El adaptador SUNAT permanece desacoplado del almacenamiento: la persistencia local vive en EF Core y el adaptador expone el punto de integracion posterior.
