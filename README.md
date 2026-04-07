# Procont Cloud Core

Bootstrap tecnico del repositorio base para Procont Cloud, un SaaS contable multi-tenant.

## Objetivo de Sprint 0

- Establecer arquitectura modular mantenible.
- Definir estandares de ingenieria y calidad.
- Habilitar entorno local reproducible con Docker.
- Configurar CI minima para validaciones tempranas.

## Estructura

- `apps/api`: ASP.NET Core Web API (.NET 8) en capas (Api, Application, Domain, Infrastructure).
- `apps/web`: Next.js + TypeScript para frontend administrativo.
- `packages/shared`: contratos y DTO compartidos.
- `infra/docker`: stack local (PostgreSQL y Redis).
- `docs`: arquitectura base y ADRs.

## Requisitos

- .NET SDK 8
- Node.js 20+
- npm 10+
- Docker Desktop

## Quickstart

1. Levantar dependencias locales:
   - `docker compose -f infra/docker/docker-compose.yml up -d`
2. Ejecutar API:
   - `dotnet run --project apps/api/src/Api/Api.csproj`
3. Ejecutar Web:
   - `npm install --workspaces`
   - `npm run dev -w apps/web`

## Calidad

- Lint web: `npm run lint -w apps/web`
- Build web: `npm run build -w apps/web`
- Build API: `dotnet build ProcontCloudCore.sln`

## Seguridad inicial

- No se almacenan secretos en el repositorio.
- Variables sensibles deben gestionarse por entorno (local con `.env` no versionado, CI con secretos de GitHub).
- Credenciales de `docker-compose` son solo para entorno local de desarrollo.
