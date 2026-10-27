# `docs/onboarding-work/browser-check-results.md`

## Purpose

Documentation covering Browser journeys executed during the onboarding audit; Isolation and setup; Executed command and result; Cleanup and scope. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/browser-check-results.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 3,280 bytes. Source hash: `cc16b962e6abb5e7449df51ca0b7b313e112d7c48a1bddd0656ea038dec33ec5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Browser journeys executed during the onboarding audit
- Line 5: Isolation and setup
- Line 17: Executed command and result
- Line 37: Cleanup and scope

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
# Browser journeys executed during the onboarding audit

Date: 27 October 2026. These are results observed during this audit, not historical CI claims.

## Isolation and setup

The checked ports 7197, 5245, 5173 and 54329 had no listener before setup. A new disposable PostgreSQL 17 container, `tutoring-onboarding-e2e-20261027`, published only `127.0.0.1:54329`. It used database `tutoring_onboarding_e2e` and the repository's `db/bootstrap-roles.sh` initialization mount to create the distinct owner and runtime roles. All administrator, owner, application and staff passwords were throwaway values supplied through process environment; none was read from the user's development database or reproduced here.

The runtime and migration connection strings explicitly selected port 54329 and this disposable database. The seed command ran in Development so the empty `giza-new` setup centre was included:

```text
dotnet run --no-build --configuration Release --project src/TutoringCentre.Api --launch-profile https -- seed
```

The restricted attempt failed with socket error 10013. The same isolated setup rerun with approved unrestricted access completed with exit code 0. The user's local development database was not selected or modified.

## Executed command and result

From `frontend/`:

```text
npm run test:e2e -- --workers=2 --reporter=line
```

````

The excerpt stops at line 24; use the original file for the remaining 17 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
