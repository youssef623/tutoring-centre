# ADR 0015: Durable jobs and outbox

- **Status:** Accepted
- **Date:** 2026-10-29

## Context

Nightly scheduling and attendance work must survive process restarts, execute per centre through
the existing authorized command pipeline, and expose operations to platform staff. Handlers also
need durable deferred commands without coupling their transaction to a later operation.

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
defaults in `hangfire`, and the existing SELECT/INSERT/UPDATE table baseline in `jobs`.
`jobs.outbox_messages` explicitly adds DELETE for purging processed rows; other jobs tables
do not inherit that wider grant.
It gets no schema CREATE and owns no Hangfire table. Runtime preparation is disabled. One storage
definition controls schema, preparation flag and the five-second queue poll interval.

Every trusted background entry point uses Application's `SystemScopeRunner` for a new async
scope, `SystemActor(centre)` and explicit correlation. Dispatch retains the normal tenant and
transaction behavior. The runner propagates exceptions and cancellation. Job classes accept
IDs/dates only and dispatch one command through the runner. Injected Hangfire execution metadata
supplies the job ID. The nightly parent lists IDs and enqueues one child per centre; platform
outbox maintenance operates only on queue metadata. Failure results are business
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

## Decision: transactional outbox

The outbox carries deferred commands for this application, not integration events with unknown
subscribers. Each stable message type has one explicit `AddOutboxMessage<TMessage, TCommand>`
mapping. The registry binds that registered command's response type once, then uses a typed
dispatcher delegate. JSON never supplies a CLR type name or selects an arbitrary command.
Duplicate type registration fails during startup configuration; unknown stored types are
dead-lettered immediately with `unknown_type`.

`IOutbox.Enqueue` adds an Infrastructure persistence row to the same DbContext as the business
change, without saving. The dispatcher saves once and commits both, or rolls both back.
Centre ID comes from the actor, correlation from its context, and timestamps from `IClock`.
An actor without a centre is a programming error. Messages must be sealed records containing
only Guid, DateOnly, DateTimeOffset, int, bool or enum properties. `OutboxRuleTests` enforces
that payload boundary; no names, credentials, phones or arbitrary strings are permitted.

`jobs.outbox_messages` deliberately has no RLS, EF tenant filter or tenant write guard: the relay
must claim work across centres. Compensating controls are identifier-only payloads, actor-stamped
centre IDs, the explicit registry, no HTTP surface, and a separate `SystemScopeRunner` tenant
scope for every delivery. The delivered command uses the ordinary dispatcher transaction and
RLS. The runtime role has exactly SELECT, INSERT, UPDATE and DELETE on this owner-created table,
without schema CREATE. The catalogue coverage test documents this exemption explicitly.

An in-process hosted relay runs only with `Outbox:RunRelay`. It claims 20 rows in a short
transaction with the specified UPDATE/SELECT `FOR UPDATE SKIP LOCKED` statement, a unique
worker ID and a 60-second lease. Idle polls wait two seconds; full batches drain immediately.
The claim commits before command dispatch. Completion uses a separate short transaction and
requires the same worker ID, so a worker whose row was reclaimed cannot acknowledge it.

A successful command is marked succeeded only after its transaction commits. A failure Result
is processed as failed with its error code and is not retried. Exceptions release the claim
with delay `min(5 seconds × 2^(attempts - 1), 10 minutes)` and dead-letter at attempt 8. Only
the exception type name is retained, never its message. Shutdown leaves unfinished leases to
expire. One structured entry per attempted message contains type, outcome, attempts and outbox
ID, never payload. Dead letters remain until an operator investigates them.

Delivery is **at least once**. A process can die after the command commits and before its
acknowledgement. The row is reclaimed and delivered again, so every mapped command must be
idempotent. The outbox does not provide exactly-once delivery.

The maintenance job runs at 03:11 Africa/Cairo, purging processed rows older than 30 days in
batches of 1,000 and warning about dead letters without deleting them. Operator requeue SQL
is documented in the [outbox runbook](../runbook.md).

Evidence against real PostgreSQL, with jobs invoked directly and a fake clock:

- `OutboxAtomicityTests.Case1_FailedCommand_RollsBackBusinessChangeAndOutbox`: case 1, rollback.
- `OutboxClaimTests.Case3_ConcurrentClaims_AreDisjointCoverPendingRows_AndSkipLockedRows`: case 3,
  competing consumers and a deliberately held lock; removing SKIP LOCKED makes the claim time out.
- `OutboxClaimTests.Case4_Lease_IsUnavailableAtThirtySeconds_ReclaimedAtSixtyOne_WithSecondAttempt`:
  case 4, lease recovery without waiting on wall-clock time.
- `OutboxRelayTests.Case8_Exception_ReleasesWithExponentialBackoff_ThenDeadLettersAtEight`:
  case 8, bounded retries and dead-lettering.
- `OutboxRelayTests.Case10_CrashAfterCommandCommit_ReclaimsAndDeliversAgain_WithOneEffect`:
  case 10, duplicate delivery with one idempotent business effect.

Operational details and test mapping: [jobs architecture](../architecture/jobs.md).
