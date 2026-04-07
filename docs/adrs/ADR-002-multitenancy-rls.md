# ADR-002: Multi-tenancy con RLS en PostgreSQL

- Estado: Propuesto
- Fecha: 2026-04-07

## Contexto

El producto requiere aislamiento por tenant con minimo riesgo de fuga de datos entre clientes.

## Decision

Adoptar estrategia de aislamiento logico con Row-Level Security (RLS) en PostgreSQL:

- Todas las tablas de negocio incluiran `tenant_id`.
- Politicas RLS filtraran por tenant activo en contexto de conexion.
- La aplicacion sera responsable de establecer el contexto de tenant por request autenticada.

## Consecuencias

- Ventaja: seguridad fuerte a nivel de base de datos.
- Ventaja: escalado horizontal eficiente sin base por tenant en etapa inicial.
- Costo: mayor disciplina de modelado y pruebas de seguridad sobre politicas RLS.
