# Tenant isolation

How the platform keeps one centre's data from leaking into another's. Defense in depth across four layers
(ADR 0006), all four of which now exist. Written from the implementation; every name and file below is
checked against the code, not the plan that produced it.

## The role model (Day 19)

The foundation the other layers depend on — not counted among layers 1–4 below, since it constrains who can
even reach the schema, before any per-row policy has a chance to run.

The API never connects to PostgreSQL as a role that can bypass row-level security or change schema.

| Role | Created by | Used by | Can |
| --- | --- | --- | --- |
| `tutoring_owner` | `db/bootstrap-roles.sh` | `MigrationRunner`, `dotnet ef` (`TutoringCentre.Api.Cli.AppDbContextFactory`) | Own the `platform`/`identity` schemas and everything in them; apply migrations. |
| `tutoring_app` | `db/bootstrap-roles.sh` | Every request, through the DI-registered `AppDbContext` (`TutoringCentre.Infrastructure.DependencyInjection`) | Exactly what `AddRuntimeRoleGrants` and later grant migrations give it: `SELECT`/`INSERT`/`UPDATE` on most tables, `DELETE` only where rows are genuinely removed rather than deactivated. No `CREATE`, no `TRUNCATE`, no `REFERENCES`, no superuser, no `BYPASSRLS`. |

Both roles are bootstrapped once, outside migrations (`db/bootstrap-roles.sh`), with passwords supplied
from the environment — `ConnectionStrings:Postgres` (runtime) and `ConnectionStrings:PostgresMigrations`
(owner) are two separate, independently validated configuration values
(`TutoringCentre.Infrastructure.Persistence.DatabaseOptions`), and the owner connection is never resolved
through the application's dependency injection container. A readiness check
(`RuntimeRolePrivilegeHealthCheck`) fails closed — `/health/ready` returns 503 — if the runtime connection
is ever a superuser, has `BYPASSRLS`, or is the owner role itself: the one assumption every later layer
depends on, checked at run time, not just at review time.

**Why this has to come first.** Row-level security (Layer 3) does not constrain a superuser or, by default,
a table's owner. Without this layer, an RLS policy could pass every test while being silently unenforceable
in production, because the connection running those tests — or running the app — was never actually
restricted by it.

## Layer 1 *(Day 22)*

Every tenant-scoped request is refused, cheaply and uniformly, before a transaction ever opens — rather than
reaching layers 2–4 and coming back empty (a query) or failing deep inside a save (a command). This is
isolation in the sense of *scope*, not row filtering: it never looks at a row, only at whether the acting
actor has a centre at all.

**The marker.** `ITenantScoped` (`TutoringCentre.Application.Common.Cqrs`) is a memberless interface a
command or query implements to declare that it operates on one centre's data. A request that runs before or
outside a tenant — login, centre selection, a platform-wide query — does not implement it; no Month 1
request does.

**The step.** `Dispatcher.CheckTenantScope` runs in both `SendAsync` and `QueryAsync`, after validation and
before `IUnitOfWork.BeginAsync` — the dispatcher is the one place every caller passes through, the seed CLI
and any future client included, so the rule cannot be bypassed by calling a handler some other way. It is a
no-op for a request that does not implement `ITenantScoped`. For one that does:

- an anonymous actor is refused with `Error.Unauthenticated("auth.not_authenticated", …)` (401) — the
  session itself is the problem, never a 404;
- any actor with no centre (`Actor.CentreId` is null) is refused with
  `Error.Forbidden("tenant.not_selected", …)` (403);
- an actor with a centre passes through unaffected.

A refused request never calls `GetRequiredService<ICommandHandler<…>>`/`IQueryHandler<…>`, never begins a
transaction, and its outcome is logged through the dispatcher's existing structured log line like any other
result. `ICurrentActor` is resolved lazily, only for a tenant-scoped request, so every other pipeline — most
of Month 1 — never needs it registered at all.

**Why here, not HTTP middleware.** The dispatcher gains no EF, HTTP or role logic: it only asks
`ICurrentActor` whether a centre is set. Placing the check in middleware would miss the seed CLI and any
future non-HTTP caller; placing it in each handler would mean one more thing to remember per use case. The
dispatcher is the layer every request already passes through for validation and the transaction boundary, so
it is also where this policy lives.

**What it does not check.** Only that a centre is present — not that the actor is still an active member of
it. Membership was already established by Month 1's centre selection and session revalidation; this step
does not re-derive it.

**The architecture guarantee.** `TenantScopeRuleTests` (`TutoringCentre.Architecture.Tests`) keeps a list of
tenant-module Application namespaces (today: `TutoringCentre.Application.Academics`) and asserts every
command or query inside one of them implements `ITenantScoped`, by implemented interface rather than
class-name suffix; the inverse guard asserts the real Month 1 namespaces (Identity, Centres, Platform) never
do. It is vacuously true until Day 24, when Subject's first use case either keeps the marker or breaks the
build.

**Where Day 28 fits.** The permission step (role-based authorization) is marked directly after this one, in
both pipelines, still before `BeginAsync` — a forbidden request must never open a transaction either way.

## Layer 2 *(Day 20)*

Every centre-scoped row is invisible to a query and unwritable by a save unless it belongs to the acting
actor's centre — enforced inside EF Core, on every type that opts in, with no per-query or per-handler code.

**The guarantee.** A query against any entity implementing `ITenantOwned`
(`TutoringCentre.Domain.Common`) returns only rows whose `centre_id` equals the acting actor's centre. An
actor with no centre (anonymous, or signed in before selecting one) sees none. A save of such an entity is
refused — the whole `SaveChangesAsync` call fails, nothing partial commits — unless every added, modified or
deleted row already belongs to that same centre, and, for a modified row, the centre itself was not touched.
Together these hold regardless of which handler or repository issued the query or the save: there is no
per-entity filter to forget and no call that opts back in by accident.

**How reads are filtered.** `TenantQueryFilterConvention`
(`TutoringCentre.Infrastructure.Persistence.Conventions`) walks the EF model once, in
`AppDbContext.OnModelCreating`, after every `IEntityTypeConfiguration` — including a test-only context's own
extra mapping — has run. For each mapped type assignable to `ITenantOwned` it attaches the filter
`entity.CentreId == CurrentCentreId`, where `CurrentCentreId` (`AppDbContext.CurrentCentreId`) is a property
read from the request's `ICurrentActor` at query-translation time, not a value captured when the model was
built: the same compiled query re-reads it on every execution, so one `AppDbContext` instance filters
correctly even if the actor is set on it after the instance already exists (the case a login request is in,
before `ActorMiddleware` sets the actor it just authenticated). The convention throws at startup if an
`ITenantOwned` type turns out to be owned or keyless, rather than silently mapping it without a filter.

**How writes are guarded.** `TenantWriteGuardInterceptor`
(`TutoringCentre.Infrastructure.Persistence.Interceptors`) runs on `SavingChanges`/`SavingChangesAsync`,
after `TimestampInterceptor`, and inspects every tracked `ITenantOwned` entry that is Added, Modified or
Deleted. It throws `TenantViolationException` — naming only the entity type, never a row or another centre's
id — the first time any of these holds:

1. The acting actor has no centre (anonymous, or a system actor explicitly scoped to none).
2. The entry's centre does not equal the actor's centre — whether that row is being inserted, updated or
   deleted.
3. The entry is Modified and its centre value itself was changed, even if the new value happens to equal
   the actor's own centre — a row cannot be reassigned into or out of a centre by editing `centre_id`.

A system actor is not special-cased: it follows the same three rules as a signed-in staff actor, scoped by
whatever centre it was given (or none). On an entity that also carries Layer 4's composite key (Task 21.6),
rule 3 is backed up by EF itself: `centre_id` is part of an alternate key once `ConfigureTenantOwned` is
applied, so EF's own change tracker refuses to mark it modified at all — the guard never gets the chance to
run, which is stronger, not weaker.

**The bypass allow-list.** Four places are allowed to run outside this filter, because each already proved
it needs to: the unit of work's one `SET TRANSACTION READ ONLY` statement
(`TutoringCentre.Infrastructure.Persistence.UnitOfWork`), the runtime-role readiness probe
(`RuntimeRolePrivilegeHealthCheck`), every EF migration under `Persistence/Migrations`, and the migration
runner that applies them (`MigrationRunner`) — all four run as `tutoring_owner` or before an actor exists,
never as a tenant-scoped request. An architecture test
(`TutoringCentre.Architecture.Tests.QueryFilterBypassTests`) reads every other production `.cs` file under
`src/` and fails the build, naming the offending file, if it calls `IgnoreQueryFilters`, `FromSqlRaw`,
`FromSql`, `SqlQuery`, `ExecuteSqlRaw` or `ExecuteSql` — the EF APIs that sidestep a global query filter or
read/write a tenant table without one at all. A second test
(`TutoringCentre.Architecture.Tests.TenantModelTests`) builds the real model and asserts every `ITenantOwned`
type carries a filter; today that runs over zero production entities (Subject, Day 23, is the first), so it
also builds a throwaway probe model to prove the convention still fires on a type it has never seen by name.

**Known gaps — why Layer 3 exists.** This layer is enforced by EF Core, inside the application process; it
is not enforced by PostgreSQL itself. Three ways it can still be missed:

- **Raw SQL.** `FromSqlRaw`, `FromSql`, `SqlQuery` and `ExecuteSqlRaw`/`ExecuteSql` do not go through EF's
  query pipeline, so the global filter never runs over them — this is an EF limitation, not a bug in the
  convention. Writing one of these against a tenant table outside the allow-list above fails the build (the
  architecture test above); if it somehow shipped anyway, it would read or write across centres with nothing
  in this layer to stop it.
- **Another process.** Anything that reaches PostgreSQL without going through `AppDbContext` — a support
  script, a different service, a future batch job — never sees this filter or guard at all; both live in
  this one C# type, not in the database.
- **A bug in the filter itself**, or in `ICurrentActor`: if `CurrentCentreId` were ever computed wrong, every
  row for every centre would be exposed the same way, uniformly, with nothing here positioned to notice.

None of these is hypothetical enough to leave uncovered: Layer 3 (row-level security, Day 21) enforces the
same `centre_id` boundary inside PostgreSQL itself, under a database session setting rather than application
code, so it still holds in exactly the cases above.

## Layer 3 *(Day 21)*

Every statement a transaction runs is filtered by PostgreSQL itself, by actual row-level security policies —
a guarantee layer 2 cannot give, because layer 2 only ever runs inside the application process.

**The setting.** `UnitOfWork.BeginAsync` (`TutoringCentre.Infrastructure.Persistence`) sets
`app.current_centre` immediately after the transaction begins — after `SET TRANSACTION READ ONLY` for a
read-only transaction, since PostgreSQL requires that to be the literal first statement, but still before
any application code runs:

```sql
SELECT set_config('app.current_centre', {0}, true)
```

The value is the acting actor's centre id as text, or an empty string when the actor has no centre — always
set explicitly, on both the command and the query pipeline, since both go through this one method. The
centre id is a bound parameter, never interpolated into the SQL text; the third argument is always `true`
(transaction-local, equivalent to `SET LOCAL`), never `false` (session-local) — a connection returned to
Npgsql's pool must never carry one request's centre into the next request that happens to reuse it.

**The setting's lifetime.** Being transaction-local, `app.current_centre` reverts whenever the transaction
ends — on `COMMIT` exactly as on `ROLLBACK` — to whatever it was before that transaction, which for a
pooled connection's next transaction is nothing. `TenantContextLifetimeTests` proves this directly: after a
request commits, a brand-new transaction on the *same* physical connection sees no centre, not the one that
just committed; after a request's command fails and rolls back, the same holds. Getting this wrong (`false`
instead of `true`) is exactly what a connection-pooled, multi-tenant web app cannot afford, and is the one
line a mutation check (flipping that argument) catches by failing that same test.

**The policy.** `TenantRowLevelSecurity.EnableTenantRowLevelSecurity(schema, table)`
(`TutoringCentre.Infrastructure.Persistence.Migrations`) — a migration helper beside Task 19.4's
`RuntimeAccessGrants`, first called by the `AddSubjects` migration (Day 23) — issues, for one table:

```sql
alter table <schema>.<table> enable row level security;
alter table <schema>.<table> force row level security;
create policy tenant_isolation on <schema>.<table> for all
    using (centre_id = nullif(current_setting('app.current_centre', true), '')::uuid)
    with check (centre_id = nullif(current_setting('app.current_centre', true), '')::uuid);
```

One policy, `FOR ALL`, the same expression on `USING` and `WITH CHECK`, no `TO` clause — so it binds every
role, including the table's own owner, not a named subset. `FORCE` is what makes that true for the owner:
without it, PostgreSQL exempts a table's owner from its own policies, which would mean `tutoring_owner` —
not a superuser, but also not restricted by RLS — could read or write across every centre. The `nullif`
guards against ever casting an empty string to `uuid` (an error): with no centre set, the comparison becomes
`centre_id = NULL`, which is never true, so an actor with no centre sees and can touch no rows. The
expression is defined exactly once, in this one static class, and reused — not duplicated — by
`PostgresFixture.CreateProbeTableAsync`, so the probe table proves the exact SQL a real migration would run.
Schema and table names are checked against a safe-identifier pattern before reaching the SQL text; a quote
or a semicolon is rejected.

**Why the test reset runs as the superuser.** `FORCE ROW LEVEL SECURITY` means even `tutoring_owner` cannot
see or delete another centre's rows without a centre set — so `PostgresFixture.ResetAsync`, which must clear
every test's data regardless of which centre(s) it touched, runs Respawn's connection as the container's
actual PostgreSQL superuser (the one role RLS never applies to at all), not as `tutoring_owner`.

**Proven today on the probe table only.** Eleven raw-SQL tests (`ProbeRawSqlRowLevelSecurityTests`), as
`tutoring_app` inside explicit transactions, assert SQLSTATEs and row counts directly against
`probe.tenant_probes`: no setting selects zero rows; the right setting selects only that centre's rows; an
insert or update naming another centre reports SQLSTATE `42501`, zero rows affected, the original row
untouched; a non-UUID setting errors rather than silently matching nothing; `tutoring_owner` with no setting
also sees zero rows (proving `FORCE`); `SET row_security = off` as `tutoring_app` errors rather than
returning unfiltered rows; the setting from a committed transaction is gone on the next one. Catalogue
queries (`ProbeRowLevelSecurityCatalogueTests`) confirm `pg_class.relrowsecurity` and `relforcerowsecurity`
are both true and `pg_policies` shows exactly the one policy described above.

**Proven on a production table since Day 23.** The same eight checks are repeated directly against
`academics.subjects` (`SubjectRawSqlRowLevelSecurityTests`), with one addition specific to that table: a
`DELETE` is refused with SQLSTATE `42501` before row-level security ever gets a chance to run, because
`tutoring_app` has no `DELETE` privilege on it at all (Task 23.3). A catalogue-driven test
(`RowLevelSecurityCoverageTests`) discovers every table with a `centre_id` column from PostgreSQL's own
catalogue and fails, naming the table, if any of them lacks forced row-level security and a complete policy —
so a future tenant table that forgets this layer fails the build rather than shipping unnoticed; its one
documented exemption is `identity.memberships`, for the reason in the Exemptions table below. The schedule's
Week 4 demo (`database/demos/week4-rls.sql`) reproduces the same four results with no application code
involved at all: `psql`, as the real runtime role, inside a transaction seeded and rolled back by the
superuser.

**Independence from Layer 2.** `TenantIsolationIndependenceTests` proves the two layers are each a complete
control, not a pair that only works together: a query that explicitly bypasses the EF filter
(`IgnoreQueryFilters`) still returns only the acting actor's rows, because RLS alone still filters it; a raw
SQL query, which EF's filter can never reach at all, is filtered the same way. In the other direction, with
the superuser connection substituted for `tutoring_app` — so RLS no longer applies — the EF filter alone
still returns only the right rows. Removing either layer in isolation, confirmed by mutation testing, breaks
only the test that depends on it.

**Known gap — why Layer 4 exists.** RLS filters which *rows* a statement can see or touch; it does not stop
a tenant-owned table from storing a foreign key that merely points at a row that exists *somewhere*, in any
centre. A single-column foreign key on a parent id is satisfied by that parent existing at all — it says
nothing about which centre it belongs to.

## Layer 4 *(Day 21)*

A composite key makes storing a cross-centre reference structurally impossible — a guarantee that holds even
for a role that bypasses row-level security entirely, which Layer 3 alone cannot give.

**The convention.** `TenantOwnedConfiguration.ConfigureTenantOwned<TEntity>(tableName)`
(`TutoringCentre.Infrastructure.Persistence.Configurations`) — called once, from inside a tenant-owned
entity's own `IEntityTypeConfiguration`, the same way every other mapping detail is declared — gives the
table: `centre_id` required; a `RESTRICT` foreign key to `platform.centres` named `fk_<table>_centres`; and
an **alternate key** (not merely a unique index — only a declared key can be the target of another entity's
composite foreign key) on `(centre_id, id)` named `ux_<table>_centre_id_id`. Any tenant-owned entity that
references another tenant-owned entity must then target that alternate key with its own composite foreign
key — `(centre_id, parent_id)` referencing `(centre_id, id)` — rather than `parent_id` alone: the database
can then only store a reference to a parent row that exists in the *same* centre as the child.

**Proven on a probe, not yet on any production table.** Month 2 has no production tenant-to-tenant
relationship, so the convention is not applied to any real table yet (Subject, Day 23, is first). A
test-only child, `probe.tenant_probe_children` (created directly by SQL, never a migration), carries a
composite foreign key `(centre_id, probe_id)` to `probe.tenant_probes(centre_id, id)`. As the PostgreSQL
superuser — deliberately: bypassing both the role model (Layer 0) and row-level security (Layer 3) entirely
— inserting a child naming Nile's centre but a probe that actually belongs to Maadi fails with SQLSTATE
`23503`, a plain foreign-key violation, proving the constraint holds regardless of who is running the query.

**The model test.** `TenantModelTests` (extended, `TutoringCentre.Architecture.Tests`) builds the real model
and asserts, for every `ITenantOwned` entity: `CentreId` exists and is required; a `RESTRICT` foreign key to
`Centre`; a key on exactly `(CentreId, Id)`; and, for every foreign key whose principal is also tenant-owned,
that the foreign key includes `CentreId` on the dependent side and the key it targets includes `CentreId` on
the principal side. Since production has no tenant-to-tenant relationship to check, a throwaway probe-and-
child pair local to the test keeps that last assertion non-vacuous — the same reason Layer 2's model test
needed its own throwaway probe. Mapping the child with a single-column foreign key instead of the composite
one fails the test, naming the exact relationship that is wrong.

## Exemptions

Six production tables carry no centre-based policy, confirmed against the schema (no migration calls
`EnableTenantRowLevelSecurity`; no configuration calls `ConfigureTenantOwned`) rather than assumed:

| Table | Why it has none |
| --- | --- |
| `platform.centres` | It is the tenant, not something scoped *by* a tenant — a row here defines a centre, it does not belong to one. |
| `identity.users` | A user can hold memberships in more than one centre (`identity.memberships`), so the user row itself cannot be scoped to a single centre. |
| `identity.user_claims`, `identity.user_logins`, `identity.user_tokens` | ASP.NET Core Identity's own tables, each keyed by user id — they inherit `identity.users`' reasoning, not centre-scoped. |
| `identity.memberships` | Deliberately not `ITenantOwned` (see the type itself): a membership is read at login, before `app.current_centre` is ever set, to show the user which centres they belong to and let them pick one. Forcing a centre-based policy on it would make centre selection impossible — reading your own memberships has to work before you have selected a centre. |

If a future table needs centre scoping and is missing from both layers, that is the bug this list exists to
catch — every exemption above has a reason checked against the code, not an assumption.
