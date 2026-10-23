# ADR 0011: Pessimistic lock for enrollment capacity

- **Status:** Draft — to be confirmed once the locking load is implemented and tested (Day 48)
- **Date:** 2026-10-23

## Context

A group has a fixed number of seats (`Group.Capacity`, 1–200), and enrolling a student must never take the
group past it (Day 47; `docs/architecture/academics.md`, "Enrollments"). The decision itself is simple and
lives in the Domain: `Group.EnsureCanEnroll(activeEnrollmentCount)` refuses with `group.full` once the count of
active enrollments reaches the capacity. The difficulty is the count. It is a fact about *other rows* — the
group's active enrollments — so the enroll use case has to read it, decide, and insert, and two requests for the
last seat can interleave between the read and the insert:

```text
Tx A                                   Tx B
COUNT active = 19                      COUNT active = 19
EnsureCanEnroll(19) ok                 EnsureCanEnroll(19) ok
INSERT                                 INSERT
COMMIT                                 COMMIT        -> 21 students in a 20-seat group
```

A transaction alone does not prevent this. Under PostgreSQL's default isolation (READ COMMITTED) each
transaction's count sees only what was committed before that statement began, and neither insert conflicts with
the other — they are different rows, so no unique index or row version catches them. The table already enforces
every rule it can express (`academics.enrollments`: one active enrollment per student and group through a
partial unique index, same-centre references through composite foreign keys, consistent end fields through
checks), but "at most N active rows for this group" is not a unique or a check constraint: it is a rule about a
count of rows, and SQL constraints are evaluated per row.

## Alternatives considered

- **An optimistic counter on the group** (an enrolled count on `groups`, guarded by the row's `xmin`; the loser
  of a race gets `concurrency.stale` and retries). It detects the race, but it turns a correct, valid request
  into a version conflict the client must resolve, at exactly the moment a popular group fills — the worst time
  to make users retry. It also stores a second source of truth for the count, next to the enrollment rows
  themselves, which can drift from them (a missed decrement on withdrawal, a manual fix in one place and not
  the other).
- **SERIALIZABLE isolation with retry** (run the enroll transaction at SERIALIZABLE; PostgreSQL's predicate
  locking aborts one of the two with `40001`, and the application retries). It is correct without any explicit
  lock, but it changes the isolation level of one use case in a pipeline where every other request runs at the
  default, needs a retry loop around the dispatcher's unit of work (which has none today, by design), and can
  abort transactions that did not actually conflict on the seat count. The cost is paid in a retry mechanism
  the rest of the system does not need.
- **An advisory lock** (`pg_advisory_xact_lock` on a key derived from the group ID). It serializes the writers
  of one group as well as a row lock does, but the lock is not tied to any row: nothing in the schema says which
  operations must take it, a hash collision between two group keys serializes unrelated groups, and row-level
  security does not apply to it. It is a parallel locking vocabulary next to the one the database already has
  for "this group's row".
- **A stored counter with a check constraint** (`enrolled_count <= capacity` on `groups`, incremented by the
  enroll use case). The check would make overbooking unstorable, and the increment itself takes a row lock. But
  it is the optimistic counter's second source of truth again, now with a constraint whose violation surfaces as
  a generic database error rather than the Domain's `group.full`, and every withdrawal, transfer and future
  correction must keep the counter exact forever.
- **A pessimistic lock on the group's row** (this decision): the use case locks the group with
  `SELECT … FOR UPDATE` before counting. Locking the enrollment rows instead would not work — the row that
  would overbook the group is the one being inserted, and it does not exist yet, so there is nothing to lock.
  The group row does exist, is the natural owner of "this group's seats", and is already guarded by row-level
  security like every other tenant row.

## Decision

Every operation that depends on a group's active enrollment count — enrolling, transferring in, changing the
capacity, archiving the group — loads the group with `SELECT … FOR UPDATE` inside the dispatcher's own
transaction (after `app.current_centre` is set, so row-level security still applies), then counts the group's
active enrollments, then lets the Domain decide (`Group.EnsureCanEnroll(count)`,
`Group.ChangeCapacity(capacity, count)`), then writes. The lock is held until the transaction commits or rolls
back.

```text
Tx A                                   Tx B
BEGIN + app.current_centre             BEGIN + app.current_centre
SELECT group ... FOR UPDATE  (granted)
                                       SELECT group ... FOR UPDATE  (waits)
COUNT active = 19
EnsureCanEnroll(19) ok -> INSERT
COMMIT  (lock released)
                                       (granted) COUNT active = 20
                                       EnsureCanEnroll(20) -> group.full -> ROLLBACK
```

The default isolation level, READ COMMITTED, is enough, and no use case changes it. Under READ COMMITTED each
statement sees the data committed before *that statement* began. Tx B's count is a new statement that runs only
after its lock is granted, and the lock is granted only after Tx A has committed — so the count already includes
Tx A's insert. SERIALIZABLE would add nothing here: the lock has already ordered the two writers.

The count is never stored. `Group` has no enrolled-count field; the active enrollment rows are the only source of
truth, and the count is passed into the Domain as a parameter by the repository that took the lock.

## Consequences

Enrollments into the same group are serialized; enrollments into different groups, and every read, are not
affected. The lock is held for one short transaction — a count over an index
(`ix_enrollments_centre_group_status`) and one insert — so the wait a second writer sees is bounded by that.
The loser of a race for the last seat gets the Domain's own `group.full` (409), the same answer as a request
that arrived after the group filled, not a version conflict or a database error.

The rule is only as strong as its callers: a write path that counts without first taking the lock reopens the
race. Every operation that reads the count for a decision must therefore go through the one locking load, and
Day 48's concurrency test (two parallel enrollments for the last seat, exactly one succeeding) is the check that
it does. An operation that locks two groups — a transfer — must take the two locks in a fixed order so that two
opposite transfers cannot deadlock; the order is settled with the implementation.

This ADR is a draft written before the implementation, on purpose: the sequence above is the design Day 48
implements against, and the status changes to Accepted once that implementation and its concurrency test exist.
