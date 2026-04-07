# Runbook Operacion Pre-Piloto

## Inicio

- Definir `PROCONT_CONNECTIONSTRINGS__POSTGRES`, `PROCONT_JWT__KEY` y secretos `PROCONT_SECRET__*` del entorno.
- Configurar `SunatSire:UseStubResponses=false` solo en QA/Piloto con salida controlada a SUNAT.
- Verificar migraciones con `dotnet ef database update --project apps/api/src/Infrastructure/Infrastructure.csproj --startup-project apps/api/src/Api/Api.csproj`.

## Verificaciones operativas

- Salud API: `GET /health`.
- Contexto de correlacion: `GET /api/v1/observability/context`.
- Export de metricas: `GET /metrics`.
- Revisar logs JSON y spans de SUNAT para `request_id` y `tenant_id`.

## Respuesta a incidentes

- Si SUNAT falla con errores transientes, confirmar incremento de `procont_sunat_retries_total` y latencia.
- Si SUNAT falla persistentemente, activar stub solo en entorno no productivo y abrir contingencia operativa.
- Si JWT rota, actualizar `JwtActiveKeyVersion` y desplegar nueva clave en secret backend antes del reinicio.

## Rollback

- Revertir despliegue a imagen anterior validada.
- Restaurar variables y referencias de secretos previas.
- Validar `/health`, login y acceso a datos por tenant.
