# Infra local

Servicios base para desarrollo local.

## Servicios

- PostgreSQL 16
- Redis 7

## Uso

1. Copiar `infra/docker/.env.example` a `infra/docker/.env`.
2. Ejecutar:
   - `docker compose -f infra/docker/docker-compose.yml --env-file infra/docker/.env up -d`
3. Verificar:
   - `docker compose -f infra/docker/docker-compose.yml ps`

## Seguridad

- No usar credenciales de desarrollo en ambientes compartidos o productivos.
- Mantener `.env` fuera de control de versiones.
