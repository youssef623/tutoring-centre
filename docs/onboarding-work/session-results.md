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
