# Backend checks executed during the onboarding audit

Date: 2026-10-27. Working directory: repository root. These are observations from this session, not copied historical documentation.

## Environment

`dotnet --info` reports SDK 10.0.401 and host/runtime 10.0.12 on Windows x64. In the restricted shell its workload-information query also prints a Service Control Manager access-denied exception. That is an environment inspection limitation, not evidence that the SDK is absent.

`docker version` reports Docker client 29.8.1, API 1.56, desktop-linux context. Restricted access to the daemon fails with permission denied on the Docker Desktop named pipe. The full test run therefore uses the approved unrestricted command environment, where Testcontainers successfully starts disposable databases.

## Build

Command: `dotnet build -c Release --no-restore`.

Restricted execution exited 1 after 5.94 seconds, with `Build FAILED`, zero warnings and zero errors and no useful diagnostic. Re-running the same command with approved unrestricted access succeeded.

Final unrestricted result:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:01:01.90
```

The build generated the OpenAPI document. During generation, runtime logs reported a failed Data Protection key-ring database read against placeholder database `openapi_generation`:

```text
Npgsql.NpgsqlException: No password has been provided but the backend requires one (in SASL/SCRAM-SHA-256)
An error occurred while reading the key ring.
```

The build continued and completed successfully. This distinguishes MSBuild diagnostics from runtime logs emitted by the document-generation host. It also contradicts the entry-point comment that generation connects to nothing.

## Tests

Command: `dotnet test TutoringCentre.slnx --no-build -c Release --logger 'trx;LogFilePrefix=onboarding' --results-directory artifacts/onboarding-tests`.

The command runs with approved unrestricted access so Testcontainers can access Docker. Current completed project results:

| Project | Passed | Failed | Skipped | Duration reported |
| --- | ---: | ---: | ---: | --- |
| Domain | 289 | 0 | 0 | 303 ms |
| Application | 489 | 0 | 0 | 434 ms |
| Architecture | 28 | 0 | 0 | 2 s |
| Infrastructure | 433 | 0 | 0 | 2 min 26 s |
| API | 634 | 0 | 0 | 19 min 16 s |

All five projects completed, overall command exit status 0: **1,873 passed, zero failed, zero skipped**. TRX files are saved in `artifacts/onboarding-tests/`. The API TRX is approximately 535 MB because it captures extensive runtime logs; it should not be committed or loaded wholesale just to obtain the already-reported runner totals. The API testhost exited at approximately 11:02 local time. The long-running suite was inspected for progress and was consuming CPU, then completed successfully; it was not terminated.

## Other checks

Command: `dotnet ef migrations has-pending-model-changes --project src/TutoringCentre.Infrastructure --startup-project src/TutoringCentre.Api --no-build --configuration Release`, with dummy runtime/migrations connection strings matching CI's design-time approach. Approved unrestricted execution exited 0:

```text
The Entity Framework tools version '10.0.12' is older than that of the runtime '10.0.13'.
No changes have been made to the model since the last migration.
```

This proves model/snapshot agreement for the built current model. It does not apply a migration or verify a deployed database. The tool/runtime version warning is an observed tooling mismatch, not a build/test failure.

No frontend check result is claimed in this file; the frontend investigator owns those commands. No full Docker image build, local production Compose stack or browser smoke run has been executed by this investigator during this audit.

Mermaid was not installed in `frontend/node_modules/mermaid` or `frontend/node_modules/@mermaid-js/parser`; neither frontend nor smoke lockfile contains Mermaid. A separate parser installation or available shared runtime is needed for real diagram parsing, rather than a regex-only syntax claim.
