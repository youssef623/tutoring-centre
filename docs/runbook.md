# Outbox operations

General deployment and recovery procedures remain in the [operations runbook](operations/runbook.md).
This procedure is for trusted operators using the migration connection as `tutoring_owner`.
Never grant the runtime role schema CREATE or owner privileges to repair queue data.

## Inspect dead letters

Connect through the existing owner connection without copying credentials into documents or
shell history. Confirm the role before reading queue metadata:

```sql
SELECT current_user; -- must be tutoring_owner

SELECT id, centre_id, type, attempts, last_error, occurred_at, dead_lettered_at
FROM jobs.outbox_messages
WHERE dead_lettered_at IS NOT NULL
ORDER BY dead_lettered_at, id
LIMIT 100;
```

Inspect identifiers and error codes/type names only. Do not print payloads to logs or tickets.
An `unknown_type` row requires the missing explicit registry mapping to be deployed first.
Other exceptions require their underlying fault to be fixed before requeueing. Confirm the
consumer is idempotent: its previous execution may already have committed.

## Requeue one reviewed row

Set the psql variable `outbox_id` to the reviewed row's UUID. Preserve the centre, payload,
type and original correlation. This grants a new attempt budget and makes the row available
immediately; the relay may deliver it again.

```sql
BEGIN;

UPDATE jobs.outbox_messages
SET dead_lettered_at = NULL,
    attempts = 0,
    available_at = now(),
    claimed_by = NULL,
    claimed_until = NULL,
    last_error = NULL
WHERE id = :'outbox_id'::uuid
  AND dead_lettered_at IS NOT NULL
  AND processed_at IS NULL
RETURNING id, centre_id, type;

COMMIT;
```

Expect exactly one returned row. Zero means the row is no longer an unprocessed dead letter;
investigate rather than broadening the WHERE clause. Do not requeue every row indiscriminately.
A business failure Result is already processed as failed and is deliberately not retried by
this procedure. Inspect the command's business outcome separately.

The relay logs type, outcome, attempts and outbox ID. The maintenance job reports the dead-letter
count and purges only processed rows older than 30 days in batches of 1,000. It never deletes
dead letters. Delivery is at least once; requeueing does not imply exactly-once execution.

The guarantee and maintenance tests exercise these states against disposable PostgreSQL.
The manual owner requeue procedure is documented for operations and was not run against
development or production data as part of implementation.
# Month 5 fresh-local checkpoint (Days 88-90)

Use a uniquely named disposable Compose project beginning `tutoring-d88-`, loopback-only PostgreSQL port, and its own named volume. Inspect `docker compose config`, container/volume labels and the exact resolved project before `down -v`. Never run a recursive/global Docker prune or reset the existing developer database. Refuse deletion if ownership/disposability is uncertain. The checkpoint evidence names the actual project and volumes used.

| # | Required observation | Current evidence/status |
|---|---|---|
| 1 | Scoped `docker compose down -v`, empty-database startup/migrations, seed twice without duplicates | Passed: task-owned tutoring-d88-checkpoint-20261031; fresh migration and both seeds exit 0; counts unchanged (6 groups / 18 students / 31 enrollments / 0 charges before HTTP setup). Seeded-upgrade migration test also passes. |
| 2 | tutoring_app without tenant sees zero portal_links, outbound_messages, agent_runs, tool_calls and charges; UPDATE/DELETE denied for tool_calls and charges | Passed: all five counts 0; all four writes permission-denied; charge RLS enabled/forced and grants exactly INSERT/SELECT. |
| 3 | Three real seeded-parent Arabic/English/Franco messages agree with attendance, with an available provider key held only in environment/user-secrets | Skipped at user request; no remote provider key is configured. Scripted HTTP checks are not real-provider evidence. |
| 4 | Fresh real recording then offline replay with equivalent logical transcripts/scores, excluding mode/date metadata | Fresh recording skipped at user request. Retained genuine recordings replay offline: 13/13 pass, both gates pass; Franco N/A. Active 13 replaces the original 15 by explicit user instruction. |
| 5 | Dashboard lists billing.monthly-charges; Development trigger twice yields one obligation per monthly key | Manual dashboard trigger skipped at user request. Registration cron/zone, child/fan-out and repeat/concurrency idempotency tests pass without a Hangfire server. |
| 6 | Search all captured fresh API/relay session logs for the actual message text, names, phones, provider key (if configured) and temporary passwords | Passed after fixing seed CLI logging: five post-fix runtime logs, 89 actual-data/credential patterns, 0 matches. Original seed logs had 21 person-data matches and are preserved privately as pre-fix evidence. Actual HTTP temporary/changed passwords and messages had zero API/relay matches. No provider key was configured. |
| 7 | Deactivate parent account, observe session end within one minute and chat 401 | Passed over real loopback HTTPS/cookies: chat returned 401 after 52,869 ms using the default 60-second cache. No browser was launched. |
| 8 | Existing Month 4 webhook script replayed five times stores one message | Unverified: this checkout contains no webhook replay script or public webhook route to execute. Existing inbound idempotency regression tests passed; they are not claimed as this manual five-replay observation. |
| 9 | Backend, frontend and actual browser regression suites green | Backend full run plus affected reruns cover all 3,228 tests; the final 17 API checks pass. Frontend 417/417 passes; build/typecheck/lint/i18n pass. Browser execution skipped at user request, not claimed green. |

Also verify two centres have no foreign charge/conversation/run visibility and editing a fee leaves existing charge amounts unchanged. The psql inspections use the existing role bootstrap and application credentials, not a privileged application connection. Inspect `\d billing.charges` and grants as part of the schema proof.

Local operator commands (run from the repository root with configuration already supplied):

```text
dotnet run --project src/TutoringCentre.Api -- migrate
dotnet run --project src/TutoringCentre.Api -- seed
dotnet run --project src/TutoringCentre.Api -- seed
dotnet run --project tests/TutoringCentre.Evaluation -- selftest
dotnet run --project tests/TutoringCentre.Evaluation -- run --mode record --out eval-record
dotnet run --project tests/TutoringCentre.Evaluation -- run --mode replay --out eval-report
```

The local `relay-once` API CLI is gated to Development/Testing and drives exactly three production relay batches; it adds no HTTP route. Browser journeys call it explicitly. Their `journey-snapshot` evaluation CLI reuses DatabaseSnapshot, prints only a content fingerprint and accepts the verified task container (or exact GitHub Actions PostgreSQL service container). Run the browser probes with the Release API/evaluation projects built and `E2E_POSTGRES_CONTAINER` configured. No test fixture or production project references Evaluation.

The repository has no `month4-journey.spec.ts`; its Month 4 proof lives in the actual attendance/card/scanner journeys. Run the supplied named selection and the complete existing suite so a nonexistent pattern cannot silently stand in for Month 4 coverage. No separate Month 5 checkpoint template was supplied; docs/operations/month5-checkpoint.md follows the existing Month 3 evidence convention.
