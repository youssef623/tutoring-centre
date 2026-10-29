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
