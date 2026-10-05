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

*To be written.*

## Layer 2 *(Day 21)*

*To be written.*

## Layer 3 *(Day 21)*

*To be written.*

## Layer 4 *(Day 22)*

*To be written.*
