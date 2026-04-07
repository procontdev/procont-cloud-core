# Sprint 2 - Reporte de entrega

## Alcance implementado

- EF Core + PostgreSQL con `ProcontDbContext`, repositorios EF, migracion inicial y seeding demo.
- Hashing robusto con BCrypt y remocion del hasher SHA-256.
- JWT y connection strings preparados para resolverse por variables de entorno prefijadas con `PROCONT_`.
- Policies por permiso activadas en endpoints de companias, contabilidad y SIRE.
- Modulo SIRE base con contratos, servicio y adaptador mock sin dependencia SUNAT.
- Tests HTTP de login, tenant resolution y asientos agregados a `Application.Tests`.

## Scripts de migracion

- Crear migracion: `dotnet ef migrations add <Nombre> --project apps/api/src/Infrastructure/Infrastructure.csproj --startup-project apps/api/src/Api/Api.csproj`
- Aplicar migraciones: `dotnet ef database update --project apps/api/src/Infrastructure/Infrastructure.csproj --startup-project apps/api/src/Api/Api.csproj`
- Levantar dependencias locales: `docker compose -f infra/docker/docker-compose.yml up -d`

## Variables de entorno relevantes

- `PROCONT_CONNECTIONSTRINGS__POSTGRES`
- `PROCONT_JWT__ISSUER`
- `PROCONT_JWT__AUDIENCE`
- `PROCONT_JWT__KEY`

## Cobertura y evidencia local

- `dotnet test ProcontCloudCore.sln --collect:"XPlat Code Coverage"`
- Archivos generados:
  - `apps/api/tests/Domain.Tests/TestResults/c16c8dca-5762-4ff0-88fc-10dcf0ffb475/coverage.cobertura.xml`
  - `apps/api/tests/Application.Tests/TestResults/2a9a9e8b-c3b5-41d8-adf2-173fa4c28291/coverage.cobertura.xml`

## Riesgos remanentes

- Los integration tests usan SQLite en memoria para velocidad; falta una corrida CI contra PostgreSQL real.
- El secreto JWT ya no esta hardcodeado en `appsettings.json`, pero aun requiere orquestacion externa en ambientes compartidos.
- El modulo SIRE es placeholder funcional; no contempla persistencia ni conciliacion tributaria.
- Aun no se implementa RLS nativo en PostgreSQL, solo filtro por `tenant_id` desde aplicacion/repositorio.
