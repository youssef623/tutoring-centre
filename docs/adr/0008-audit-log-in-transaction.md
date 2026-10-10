# ADR 0008: Audit log written in the command's own transaction

- **Status:** Accepted
- **Date:** 2026-10-10

## Context

A role change, a subject rename or a centre settings change needs a durable record of who changed what and
when, with the before and after values — and that record has to exist whenever the change itself exists,
with nothing that can commit the data change but lose its audit row, or the reverse. The record also must
never block or meaningfully slow down the everyday commands it watches, and it must not give any handler a
new thing to remember.

## Alternatives considered

- **Explicit handler calls** (each handler that changes an audited entity calls an `IAuditLog.Record(...)`
  port itself) keeps the mechanism visible at the call site, but puts the audit allow-list's enforcement in
  every handler's hands rather than in one place: a handler that forgets the call, or a new handler for a
  new command on an already-audited entity, silently produces no audit row, and nothing fails the build to
  say so. It also means Application code has to know field-level detail (what counts as a change, what
  values are allowed) that belongs to the audit concern, not the use case.
- **Database triggers** (a PostgreSQL trigger on `UPDATE`/`INSERT`/`DELETE` of an audited table, building the
  audit row from `OLD`/`NEW`) genuinely cannot be bypassed by any application-layer mistake, and would catch
  a write made outside the application entirely. It costs real things this product does not want yet: audit
  logic in SQL instead of C#, no access to the request's correlation id or a "who is the actor" concept that
  exists only in the application process (the database has no idea which staff user issued a statement), and
  a second place (alongside `AuditPolicy`) to review and test whenever the allow-list changes.
- **An event stream** (handlers publish domain events; a separate consumer, in-process or out, writes the
  audit row from them) decouples the audit writer from the command pipeline and would suit audit data meant
  to also drive other side effects (notifications, search indexing) later. It is not what this product needs
  today, and it reopens exactly the atomicity problem this decision exists to close: either the event is
  published and handled inside the same transaction as the change (in which case it has bought nothing an
  interceptor does not already give, at more machinery) or it is handled afterward, outside that
  transaction, in which case a crash or a failure between the two can commit the change with no audit row,
  or vice versa.
- **A save-time interceptor in the command's own transaction** (this decision) adds the audit row to the
  same `SaveChanges` call the command's handler already triggers, through `DbContext.ChangeTracker`, with no
  handler code and no separate write. It costs an EF-specific mechanism (interceptors) and a dependency on
  the change tracker correctly reporting original and current values, but buys atomicity for free: there is
  only one `SaveChanges`, so there is no "and then also audit it" step that a crash, a timeout or a rolled-
  back transaction can land between.

## Decision

`AuditInterceptor` runs on `SavingChanges`/`SavingChangesAsync`, after `TimestampInterceptor` and before
`TenantWriteGuardInterceptor`, and adds one `AuditEntry` per audited, allowed change it finds already on the
change tracker — before the save those changes are part of, not after. See `docs/architecture/audit.md` for
the full flow, the allow-list and what it does not cover.

## Consequences

Atomicity is structural, not something a test merely confirms today and a future change could quietly break:
a handler that changes a subject and then fails writes neither the change nor an audit row, because the
audit row was only ever added to the same batch the failed save never reaches. A unique-constraint race's
loser writes neither, for the same reason — its whole transaction, not just its data change, is what fails.
Both are proven in `AuditTrailTests`, and a mutation check (moving the interceptor's hook to
`SavedChanges`/after the save) breaks that proof directly, rather than needing a slower, harder-to-trust
end-to-end check for something a type signature could have caught.

The audit mechanism is coupled to EF Core's change tracker: anything that writes to `audit.audit_entries`'
source tables outside `AppDbContext` (there is nothing today, by the existing tenant-isolation architecture)
would bypass it silently, the same known gap Layer 2 of tenant isolation already documents and Layer 3
backs up for row visibility — there is no equivalent backup layer for audit coverage specifically, because a
database trigger was the alternative that would have provided one, and was rejected above for this slice.
