# ADR 0006: Tenant isolation as defense in depth

- **Status:** Proposed — context and decision drafted (Day 19); completed once row-level security and the
  tenant session context land (Day 21).
- **Date:** 2026-10-05

## Context

Every centre's data lives in the same database, in the same tables, distinguished only by a `centre_id`
foreign key (`platform.centres`, `identity.memberships`, and every centre-scoped table Month 2 adds after
it). A single query missing a `WHERE centre_id = ...` clause — a bug in a handler, a read service someone
forgets to scope, a future endpoint written in a hurry — would silently return or modify another centre's
data. Application-layer discipline (remembering to filter every query) is necessary but cannot be the only
safeguard: it fails silently, it cannot be tested from outside the code that might get it wrong, and it
offers nothing once a caller reaches the database some other way (a raw migration, a support script, SQL
injection). Tenant isolation needs at least one layer that holds even when the application code above it
does not.

## Decision

Tenant isolation is layered, not a single check: each layer is a complete control on its own, verified
independently, so that a gap in one does not silently remove the property the whole stack depends on.

**Layer 0 — least-privilege database roles (Day 19, this slice).** The API never connects to PostgreSQL as
a role that can bypass row-level security or change schema. `tutoring_owner` owns the schemas and runs
migrations; `tutoring_app` is the only login the running application ever uses, and holds exactly the
object grants later migrations give it — nothing more. This is the foundation the other layers depend on:
row-level security (Layer 2) does not constrain a superuser or, by default, a table's owner, so without this
layer, every later isolation guarantee would be provable in tests and false in production the moment
anything ran as the wrong role. A readiness check (Day 19) fails the instance closed if that assumption is
ever violated at runtime.

- Layer 1 — *(Day 20)*
- Layer 2 — *(Day 21)*
- Layer 3 — *(Day 21)*
- Layer 4 — *(Day 22)*

## Consequences

Not yet assessed beyond Layer 0 (see `docs/architecture/tenancy.md`). Recorded once Layers 1–4 are decided.
