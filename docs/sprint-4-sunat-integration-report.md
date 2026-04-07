# Sprint 4 - SUNAT Integration Report

- Adapter `SunatSireAdapter` now uses real HTTP flow with token acquisition, request timeout, exponential retry and sanitized request/response tracing.
- Request correlation includes `request_id` and `tenant_id` in logs, activity tags and outbound headers for SUNAT calls.
- Stub mode remains available through `SunatSire:UseStubResponses=true` for local development and integration tests without external dependency.
- Test evidence in this repo is based on build and integration coverage over the real adapter path shape; external SUNAT sandbox execution still requires pilot credentials and controlled network access.

## Technical scope

- Auth endpoint configured with `SunatSire:AuthPath` and secret-backed credentials.
- Business endpoints configured with `SunatSire:PropuestasPath` and `SunatSire:ContabilizarPath`.
- Failures classify transient HTTP/network errors for retry and persist sanitized diagnostics.

## Evidence available

- `dotnet build ProcontCloudCore.sln --configuration Release` green.
- Adapter implementation: `apps/api/src/Infrastructure/Sire/SunatSireAdapter.cs`.
- Exception contract: `apps/api/src/Infrastructure/Sire/SunatSireException.cs`.

## Pending external validation

- Execute against SUNAT test environment with issued credentials.
- Capture request ids, timestamps and redacted payload evidence for pilot dossier.
- Confirm final endpoint contracts if SUNAT publishes changes in sandbox before pilot cutover.
