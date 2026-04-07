# ADR-001: Stack Backend Base

- Estado: Aceptado
- Fecha: 2026-04-07

## Contexto

Se requiere una base backend robusta para un SaaS multi-tenant, con buena mantenibilidad y soporte de ecosistema empresarial.

## Decision

Usar ASP.NET Core Web API en .NET 8 con estructura en capas:

- Api
- Application
- Domain
- Infrastructure

## Consecuencias

- Ventaja: productividad y soporte de largo plazo (LTS).
- Ventaja: integracion nativa con pipeline de CI en GitHub Actions.
- Costo: estandarizar practicas DDD/CQRS en siguientes sprints para evitar logica en capas incorrectas.
