# Deudas tecnicas abiertas - Sprint 2

| Prioridad | Deuda | Objetivo |
| --- | --- | --- |
| Alta | Reemplazar authorization handler con cache de permisos por request para reducir queries repetidas. | 2026-04-19 |
| Alta | Migrar integration tests HTTP a PostgreSQL real con contenedor efimero en CI para cubrir Npgsql end-to-end. | 2026-04-21 |
| Media | Incorporar RLS real en PostgreSQL y validaciones de aislamiento desde DB. | 2026-04-26 |
| Media | Persistir propuestas/resultados SIRE en tablas propias cuando se defina el modelo tributario. | 2026-04-28 |
| Media | Externalizar secretos a secret manager productivo y rotacion automatizada de JWT signing keys. | 2026-04-30 |
