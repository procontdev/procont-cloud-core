# Sprint 3 Open Technical Debt

- El adaptador `SunatSireAdapter` sigue en modo stub funcional; falta contrato HTTP/SOAP real con SUNAT.
- La aplicacion establece `app.current_tenant` por interceptor EF; falta cobertura completa para accesos SQL directos fuera de EF.
- Observabilidad aun no exporta OpenTelemetry a collector externo.
- Manejo de secretos usa configuracion por entorno y metadatos declarativos; falta integrar vault productivo.
- Faltan pruebas de carga y perfiles de indices sobre tablas multi-tenant grandes.
