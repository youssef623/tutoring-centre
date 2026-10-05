# Tenant isolation

How the platform keeps one centre's data from leaking into another's. Defense in depth across four layers
(ADR 0006), of which only the first exists yet — the rest are filled in as Days 20–22 build them. Written
from the implementation; every name and file below is checked against the code, not the plan that produced
it.

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

**Why this has to come first.** Row-level security, whichever layer below ends up adding it, does not
constrain a superuser or, by default, a table's owner. Without this layer, an RLS policy could pass every
test while being silently unenforceable in production, because the connection running those tests — or
running the app — was never actually restricted by it.

## Layer 1 *(Day 20)*

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
whatever centre it was given (or none).

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

**Known gaps — why Layer 2 exists.** This layer is enforced by EF Core, inside the application process; it
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

None of these is hypothetical enough to leave uncovered: Layer 2 (row-level security, Day 21; ADR 0006)
enforces the same `centre_id` boundary inside PostgreSQL itself, under a database session setting rather
than application code, so it still holds in exactly the cases above.

## Layer 2 *(Day 21)*

*To be written.*

## Layer 3 *(Day 21)*

*To be written.*

## Layer 4 *(Day 22)*

*To be written.*
