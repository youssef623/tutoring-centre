# `docs/operations/month3-local-verification.md`

## Purpose

Documentation covering Month 3 local verification checklist (Day 54); The instance; Automated checks (Claude Code); Checks that need your eyes; Result. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../operations/month3-local-verification.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 56 lines and 6,173 bytes. Source hash: `21d6e429e4f72b534cecb1457baa98596124815b656969142abf561037a9f5b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Month 3 local verification checklist (Day 54)
- Line 8: The instance
- Line 24: Automated checks (Claude Code)
- Line 40: Checks that need your eyes
- Line 54: Result

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Month 3 local verification checklist (Day 54)

The end-of-Month-3 checks, walked on a freshly seeded **local** instance. There is no deployed smoke test: no cloud
deployment exists (`docs/architecture/deployment.md`), and the Month 2 smoke-test script and runbook are not used
here. A smoke test asks "is it up and basically right"; this checklist is that, walked by hand once. The acceptance
test, "does it do what we promised", is the `full-centre-setup` journey (`frontend/e2e/full-centre-setup.spec.ts`).

## The instance

Built on 2026-10-26 from branch `feature/m3-d54-month3-checkpoint`, with no leftover data:

1. `docker compose down -v`, then `docker compose up -d` (a new, empty `pgdata` volume; `db/bootstrap-roles.sh`
   creates `tutoring_owner` and `tutoring_app`).
2. `dotnet run --project src/TutoringCentre.Api -- migrate`: exit 0. The five Month 3 migrations are applied:
   `20261012053358_AddStudentsAndParents`, `20261017042944_AddTeachers`, `20261018053631_AddGroups`,
   `20261023174541_AddEnrollments`, `20261024105240_AddSchedule`.
3. `dotnet run --project src/TutoringCentre.Api -- seed`: exit 0. Per centre (subjects, teachers, groups,
   students, parents, enrollments, rules, sessions): Nile 3, 2, 4, 12, 9, 25, 6, 24; Maadi 2, 1, 2, 6, 6, 6, 2, 8;
   `giza-new` all zero (Development only, one owner). Six users in Development (the five demo accounts and the
   `giza-new` owner). Every rule starts on the seed day, 2026-10-26. A second seed run (done on an earlier fresh
   volume the same day) changed no count.
4. `dotnet run --project src/TutoringCentre.Api --launch-profile https`; `GET /health/ready` answered `Healthy`.

## Automated checks (Claude Code)
```

The excerpt stops at line 24; use the original file for the remaining 32 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
