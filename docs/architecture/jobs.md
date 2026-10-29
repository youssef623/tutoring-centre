# Background jobs

## Host and storage

Hangfire runs inside the API process. `Jobs:RunServer` enables the server; `Jobs:WorkerCount`
defaults to 2. Development enables it, base configuration disables it, and test hosts explicitly
disable it. CLI commands and build-time OpenAPI generation never start a server. There is one
deployable, no separate worker service and no concrete or recurring jobs yet (Day 68).

`JobsOptions` is validated at startup. Worker count must be positive, `DashboardPath` must be an
absolute non-root path without a trailing slash, query or fragment, and `OperatorEmails` must
contain email addresses. Defaults are `/ops/jobs` and an empty operator list. Storage reuses
`ConnectionStrings:Postgres` and `ConnectionStrings:PostgresMigrations`; no new secret exists.

`JobsStorage.Create` defines schema `hangfire`, a five-second queue poll interval and the schema
preparation flag for both installer and runtime. The explicit `migrate` CLI applies EF migrations
as `tutoring_owner`, then constructs storage with preparation enabled on that same owner
connection. This creates/upgrades Hangfire's own tables. Failure is not silently degraded into
a successful installation. Runtime storage uses `tutoring_app` with preparation disabled.
The API and seed command never migrate at startup; run `migrate` before either on a new database.
Test fixtures call the same migration runner, and CI migrates explicitly before seeding E2E data.

## Privileges

`AddJobsInfrastructure` creates `hangfire` and `jobs`, both owned by `tutoring_owner`.
It grants `tutoring_app` USAGE on both schemas and no CREATE. It explicitly revokes CREATE
from both PUBLIC and `tutoring_app` on these schemas.

| Schema | Default privileges for objects created by `tutoring_owner` |
| --- | --- |
| `hangfire` tables | `tutoring_app`: SELECT, INSERT, UPDATE, DELETE |
| `hangfire` sequences | `tutoring_app`: USAGE, SELECT |
| `jobs` tables | Existing `GrantRuntimeAccess` baseline: SELECT, INSERT, UPDATE |

The wider table/sequence privileges are confined to Hangfire metadata. `jobs` is empty until the
Day 69 outbox migration; it has no DELETE or sequence default grant. Future tenant tables still
need their own forced row-level security and tenant-key constraints. Default privileges apply
to future owner-created objects; they do not grant runtime DDL or ownership.

`HangfireStorageTests` checks table and schema ownership, sequence access, runtime metadata DML,
SQLSTATE 42501 for CREATE in both schemas, and enqueue through runtime storage. The enqueue
test has no server. Migration installation is idempotent; normal application start does no DDL.

## Actor and correlation

Application owns `ISystemScopeRunner` and `SystemScopeRunner`. Each `RunAsync` call creates
a fresh async DI scope, sets `SystemActor(centreId)` (null for trusted platform work), sets the
scope's correlation context, invokes the work with scoped services and cancellation, then
disposes the scope. It never catches exceptions or reuses the caller's context. The seed CLI
uses it with the same command groups and idempotent outcomes as before.

A job supplies the trusted centre and `job:<id>` correlation. Dispatch inside the callback uses
the normal validation, tenant, permission and transaction pipeline. The transaction sets
`app.current_centre`; handlers and audit use the same actor model as HTTP. Never derive the
centre from untrusted input. Runner tests prove caller isolation, distinct/disposed scopes,
correlation, cancellation and exception propagation. The Infrastructure runner test dispatches
a real query and reads back Nile's ID from PostgreSQL's transaction setting.

## Job convention and failure

Keep one class per job in Infrastructure. `ExecuteAsync` takes only IDs, dates and a cancellation
token. Its body calls the runner and dispatches one command. Never serialize entities, DTOs,
names, phone numbers or credentials as job arguments. The convention also lives beside the
job infrastructure in `src/TutoringCentre.Infrastructure/Jobs/README.md`.

A failure `Result` is a completed business outcome: log the error code without arguments and
return normally. An exception propagates to Hangfire. The global retry policy has `Attempts=3`:
three retries after the initial execution, delayed 10, 20 and 40 seconds. Jobs must tolerate
re-execution; persistence is durable, not an exactly-once execution guarantee.

`JobLoggingFilter` emits start, success and failure entries with job ID and job type only.
It never logs arguments, the serialized job, exception text or personal data. Retry-event
exception logging is disabled on the global retry filter. Existing dispatcher logging remains
request-name/outcome logging; no dispatcher changes are needed.

## Platform operators and dashboard

`IOperatorDirectory.IsOperatorAsync` reads the user's normalized email through Identity and
compares it with normalized `Jobs:OperatorEmails`. It never reads centre roles to make the
operator decision. Development lists a synthetic seeded owner for local use; the base list is
empty. A centre Owner absent from the list is not an operator. Listed users need no selected
centre, because the dashboard exposes cross-centre operational metadata.

The dashboard maps at `Jobs:DashboardPath` (default `/ops/jobs`). An authenticated cookie session
and the asynchronous platform-operator policy protect every dashboard route and asset. Signed
out returns 401; signed in but unlisted returns 403. Hangfire's default local-request shortcut is
disabled. The endpoint inventory classifies the dashboard as **operator-only**.

Outside Development the dashboard is read-only: Hangfire hides actions and the endpoint refuses
non-GET/HEAD requests with 403. In Development, actions retain Hangfire's antiforgery validation.
`DashboardTests` proves the gate, normalized allowlist, role independence, non-Development POST
refusal, both `/api/me` flag values, and absence of a test-host Hangfire server.

`GET /api/me` adds `isOperator`. The user menu shows Background jobs only for true and opens
`/ops/jobs` in a new tab. This flag is a UI hint; server authorization decides access. Vite
proxies `/ops` to the existing API origin, preserving cookie authentication. Both languages
include the label and component tests cover both flag values.

See [ADR 0015](../adr/0015-jobs-and-outbox.md) for the framework and hosting decision. Concrete
nightly jobs follow on Day 68; outbox delivery semantics are not defined by this foundation.
