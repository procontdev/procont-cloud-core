# Arquitectura base

## Principios

- Modularidad por contexto (evitar acoplamiento entre capas).
- Multi-tenant desde el inicio del dise\u00f1o.
- Seguridad por defecto (config por entorno, secretos fuera del repo).
- Observabilidad y operacion reproducible.

## Estructura

- `apps/api`: backend principal en .NET 8.
- `apps/web`: frontend de operacion y administracion.
- `packages/shared`: contratos de integracion y DTO.
- `infra`: stack local y activos de despliegue inicial.

## Multi-tenant (base)

- Identificador de tenant como metadato obligatorio en requests autenticadas.
- Persistencia orientada a aislamiento logico por tenant.
- ADR-002 define estrategia inicial de Row-Level Security para PostgreSQL.

## Seguridad y secretos

- Local: variables en `.env` no versionado.
- CI/CD: secretos administrados por GitHub Actions Secrets.
- Produccion: secretos en gestor dedicado (pendiente en Sprint 1).

## Roadmap tecnico (inicial)

1. Sprint 1: nucleo multi-tenant + IAM base.
2. Sprint 2: modulo contable habilitador + auditoria.
3. Sprint 3: trazabilidad de eventos + observabilidad.
