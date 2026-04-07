# Sprint 3 E2E Report

## Alcance cubierto

- Login JWT con resolucion de tenant por `X-Tenant-Code`.
- Flujo contable base: consulta de catalogos y creacion de asientos.
- Flujo SIRE base: listado de propuestas persistidas y contabilizacion con resultado almacenado.
- Verificacion de aislamiento RLS en PostgreSQL real cambiando `app.current_tenant`.
- Endpoint de observabilidad con correlacion `request/tenant` y metadatos de proveedor de secretos.

## Entorno de prueba

- Tests de integracion sobre PostgreSQL real via Testcontainers (`postgres:16-alpine`).
- CI GitHub Actions con servicio PostgreSQL real y variables `PROCONT_*` para secretos.

## Evidencia esperada CI

- `dotnet ef database update` aplica migraciones sobre PostgreSQL.
- `dotnet test ProcontCloudCore.sln --configuration Release --no-build --collect:"XPlat Code Coverage"` ejecuta regresion de auth, tenant, accounting y SIRE.

## Performance basica

- Escenario base probado: bootstrap + auth + lectura de propuestas + contabilizacion + consulta RLS.
- Riesgo actual: no hay benchmarking automatizado ni metricas exportadas a backend externo; solo tags y scopes locales.
