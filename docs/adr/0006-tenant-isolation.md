# ADR 0006: Tenant isolation as defense in depth

- **Status:** Accepted — Layers 0, 2, 3 and 4 implemented and proven (Days 19–21); Layer 1 (request-level
  tenant scoping) lands Day 22, tracked in `docs/architecture/tenancy.md`.
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

## Alternatives considered

A shared schema was chosen over the two usual alternatives for multi-tenant data:

- **Schema-per-tenant** (one PostgreSQL schema per centre, same tables repeated in each) isolates tenants
  structurally — a query literally cannot reach another schema without being told to — but migrations,
  connection routing and backups all multiply by the number of centres, and PostgreSQL has no efficient way
  to query across schemas for anything centre-spanning (a platform-wide report, a support tool). It does not
  suit a product whose centre count is expected to grow past what per-schema migrations stay manageable for.
- **Database-per-tenant** isolates further still — a connection to one tenant's database cannot touch
  another's at all, even under a compromised credential — at the cost of a connection (or connection pool)
  per centre, migrations that must run once per database, and no cross-tenant query path at all without an
  external aggregation layer. It is the right choice when tenants need independent scaling, independent
  backup/restore schedules, or regulatory data residence per tenant; this product needs none of those yet.
- **Shared schema with defense in depth** (this decision) keeps one set of tables, one migration path and
  one connection pool, and instead stacks independent controls — role privilege, application-level
  filtering, database-level row security, and structural foreign-key integrity — so that no single layer is
  trusted alone. It costs more design and test effort per tenant-owned table than schema- or
  database-per-tenant would, in exchange for a single operational surface. If centre-level data residence or
  independent scaling becomes a real requirement, schema- or database-per-tenant remains available later:
  this decision does not foreclose it, since every tenant-owned table already carries an explicit
  `centre_id`.

## Decision

Tenant isolation is layered, not a single check: each layer is a complete control on its own, verified
independently, so that a gap in one does not silently remove the property the whole stack depends on.

**Layer 0 — least-privilege database roles (Day 19).** The API never connects to PostgreSQL as a role that
can bypass row-level security or change schema. `tutoring_owner` owns the schemas and runs migrations;
`tutoring_app` is the only login the running application ever uses, and holds exactly the object grants
later migrations give it — nothing more. This is the foundation the other layers depend on: row-level
security (Layer 3) does not constrain a superuser or, by default, a table's owner, so without this layer,
every later isolation guarantee would be provable in tests and false in production the moment anything ran
as the wrong role. A readiness check (Day 19) fails the instance closed if that assumption is ever violated
at runtime.

**Layer 1 — request-level tenant scoping *(Day 22)*.** Not yet implemented; tracked in
`docs/architecture/tenancy.md`.

**Layer 2 — EF Core query filter and write guard (Day 20).** Every entity implementing `ITenantOwned` is
automatically filtered to the acting actor's centre on every read, and refused on every write that does not
already belong to that centre, with no per-query or per-handler code to remember. Enforced entirely inside
the application process — see `docs/architecture/tenancy.md` for what that does and does not cover.

**Layer 3 — PostgreSQL row-level security (Day 21).** The same `centre_id` boundary, enforced by PostgreSQL
itself rather than application code, driven by a `SET LOCAL`-style session setting (`app.current_centre`)
that `UnitOfWork.BeginAsync` sets, parameterised, on every transaction. Holds even if Layer 2 is bypassed or
buggy, even against raw SQL, even against another process reaching the same database — see
`docs/architecture/tenancy.md`.

**Layer 4 — composite-key tenant integrity (Day 21).** Every tenant-owned table carries a `(centre_id, id)`
key, and any reference between two tenant-owned tables is a composite foreign key on that pair, not a plain
id. This is what stops a cross-centre reference from being *storable* at all, including by a role that
bypasses row-level security entirely — a guarantee Layer 3 alone cannot give. See
`docs/architecture/tenancy.md`.

## Consequences

Layers 0, 2, 3 and 4 are implemented and proven against real PostgreSQL, each independently (removing any
one layer in isolation fails only the tests that depend on it) — see `docs/architecture/tenancy.md` for the
tests, the exemption list, and what each layer still does not cover on its own. No production table uses
Layer 2, 3 or 4 yet: Month 2 has not added a tenant-owned table (Subject, Day 23, is first), so all four are
proven on test-only probe tables today, not on production data. Layer 1 remains open, due Day 22.
