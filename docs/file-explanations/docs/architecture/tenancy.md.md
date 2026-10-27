# `docs/architecture/tenancy.md`

## Purpose

Documentation covering Tenant isolation; The role model (Day 19); Layer 1 *(Day 22)*; Layer 2 *(Day 20)*; Layer 3 *(Day 21)*. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/tenancy.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 326 lines and 25,863 bytes. Source hash: `2be0bf14d2eb6124b72ca9d834e157651cf4c63846c4ac227f88e160c115e20a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 40](../../../architecture/tenancy.md#L40): `**The marker.** ITenantScoped (TutoringCentre.Application.Common.Cqrs) is a memberless interface a`
- [Line 73](../../../architecture/tenancy.md#L73): `command or query inside one of them implements ITenantScoped, by implemented interface rather than`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
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
```

The excerpt stops at line 24; use the original file for the remaining 302 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
