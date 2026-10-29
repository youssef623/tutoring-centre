# ADR 0015: Durable jobs and outbox

- **Status:** Draft (Day 67; outbox decision completed on Day 69)
- **Date:** 2026-10-29

## Context

Nightly scheduling and attendance work must survive process restarts, execute per centre through
the existing authorized command pipeline, and expose operations to platform staff. A later
outbox must deliver committed events without coupling a business transaction to a remote call.
This draft decides the job foundation only; outbox schema and delivery semantics remain open.

## Alternatives considered

- **Hangfire with PostgreSQL.** Durable queues, retries and an operational dashboard reuse the
  existing database. Storage needs third-party schema installation and careful runtime privileges.
- **Cron plus CLI.** Small application surface and useful for simple schedules, but durable retry,
  overlapping runs, execution history and operator visibility become separate operational work.
- **Quartz.** Durable scheduling and rich trigger semantics, but the current work needs a command
  queue and ready operator dashboard rather than advanced calendars. Adding equivalent operational
  visibility would increase today's scope.

## Decision (job foundation)

Use centrally pinned Hangfire packages only in Infrastructure and the API composition edge.
Domain and Application have no Hangfire reference, attributes or job-framework types. Keep the
server inside the API process behind `Jobs:RunServer`, with two workers by default. This keeps
one deployable; a later separate host can reuse the same job classes without changing use cases.

Install storage only through the explicit owner migration path. EF creates `hangfire` and `jobs`
as `tutoring_owner`; the migrate step then prepares Hangfire's tables on the owner connection.
The runtime role gets schema USAGE, table SELECT/INSERT/UPDATE/DELETE and sequence USAGE/SELECT
defaults in `hangfire`, and only the existing SELECT/INSERT/UPDATE table baseline in `jobs`.
It gets no schema CREATE and owns no Hangfire table. Runtime preparation is disabled. One storage
definition controls schema, preparation flag and the five-second queue poll interval.

Every trusted background entry point uses Application's `SystemScopeRunner` for a new async
scope, `SystemActor(centre)` and explicit correlation. Dispatch retains the normal tenant and
transaction behavior. The runner propagates exceptions and cancellation. Job classes accept
IDs/dates only and dispatch one command through the runner. Failure results are business
outcomes; exceptions receive three retries at 10, 20 and 40 seconds. Logs contain execution ID
and job type, never arguments or serialized personal data.

The dashboard requires cookie authentication plus an operator allowlist checked by the
Application port `IOperatorDirectory`. Identity-normalized email defines membership in that
list; tenant roles never do. The dashboard is read-only outside Development. `/api/me` exposes
the operator flag for a translated menu link, while the server independently enforces access.
No default local-request authorization is trusted.

## Consequences

- Runtime has no DDL rights, including when a third-party storage package upgrades. Deployment
  must run migrate before starting the API or seeding a fresh database.
- In-process workers share API resources and deployment lifetime. Two workers bound concurrency;
  durable storage makes interrupted work recoverable. Exactly-once execution is not promised.
- The dashboard sees metadata across centres, so ordinary centre administrators cannot access it.
  Production actions are refused even for operators.
- Application remains independent of the job framework. Seed and future non-HTTP callers share
  actor setup instead of rebuilding ambient context independently.
- PostgreSQL integration tests prove grants, ownership and enqueue. Runner and dashboard tests
  prove scope/tenant isolation and the platform gate. Test hosts never run a job server.

## Outbox decision pending (Day 69)

The empty `jobs` schema reserves the module boundary only. Event serialization, transaction
atomicity, leasing, retries, retention and delivery idempotency are not decided here. Concrete
jobs and recurring registration arrive on Day 68; this draft introduces neither.

Operational details and test mapping: [jobs architecture](../architecture/jobs.md).
