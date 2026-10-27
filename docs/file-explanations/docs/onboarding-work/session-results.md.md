# `docs/onboarding-work/session-results.md`

## Purpose

Documentation covering the repository. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/session-results.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 2,231 bytes. Source hash: `595f237206f30fe8fa8c6a5b1213ab535686d3a48394ddb848e2c85506a3491b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../onboarding-work/session-results.md#L16): `Backend integration checks used disposable Testcontainers databases with Docker access. They did not reset the user's development database. The exact backend command record is [retained here](onboarding-work/backend-check-results.md). Frontend rerun settings are material to interpreting its result: the bounded-worker pass does not turn the earlier default-concurrency run into a pass. Browser journey execution is tracked separately in the browser verification record when available. Production image/cloud smoke execution is not implied by the successful application build.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
| Check executed in this session | Actual result |
| --- | --- |
| `dotnet build -c Release --no-restore` | Passed; zero compiler warnings/errors. OpenAPI generation logged a Data Protection database-read error, documented separately. |
| `dotnet test TutoringCentre.slnx --no-build -c Release` | Passed: 1,873 cases; Domain 289, Application 489, Architecture 28, Infrastructure 433, API 634; zero failed/skipped. |
| `dotnet ef migrations has-pending-model-changes ... --no-build --configuration Release` | Passed; no pending model changes. Local tool 10.0.12 reports it is older than runtime 10.0.13. |
| Frontend `npm run typecheck` | Passed. |
| Frontend `npm run build` | Passed; Vite 8.3.2 compiled 2,695 modules. |
| Frontend `npm run i18n:check` | Passed for fifteen namespaces. |
| Frontend `npm run lint` | Exited successfully: zero errors, forty-two fast-refresh warnings. |
| Frontend `npm run test:ci` with default worker concurrency | 254 passed, 24 failed; failures included timeouts. This is recorded as an unsuccessful run, not hidden by the rerun. |
| Frontend `npm run test:ci -- --maxWorkers=2` | Passed: 43 files, 278 cases; 127.06 seconds. |
| Frontend `npm run test:e2e -- --workers=2 --reporter=line` | Passed: nineteen Chromium journeys in 1.6 minutes, isolated disposable database and test-owned servers; no failed/retried case reported. |
| Markdown target/anchor/source-line validation | Passed for the assembled overview and all per-file guides; exact final counts are in the validation report. |
| Mermaid parsing | All twenty overview diagrams parsed with the installed Mermaid parser in a DOM-capable Node environment. |

Backend integration checks used disposable Testcontainers databases with Docker access. They did not reset the user's development database. The exact backend command record is [retained here](onboarding-work/backend-check-results.md). Frontend rerun settings are material to interpreting its result: the bounded-worker pass does not turn the earlier default-concurrency run into a pass. Browser journey execution is tracked separately in the browser verification record when available. Production image/cloud smoke execution is not implied by the successful application build.
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
