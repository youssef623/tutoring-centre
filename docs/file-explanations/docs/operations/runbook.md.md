# `docs/operations/runbook.md`

## Purpose

Documentation covering Runbook; 1. Deploy; 2. Roll back to the previous revision; confirm, then to return:; 1. Build an image from the previous commit without touching the working tree.. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../operations/runbook.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 214 lines and 12,697 bytes. Source hash: `99a115ab57b1f60406c6da469841ca83d97a0c7ffa9cc26a4aa4ec93737f0a1f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Runbook
- Line 11: 1. Deploy
- Line 30: 2. Roll back to the previous revision
- Line 41: confirm, then to return:
- Line 51: 1. Build an image from the previous commit without touching the working tree.
- Line 55: 2. Stop the current revision, start the previous one under the same network alias Caddy expects.
- Line 62: 3. Confirm the older SHA.
- Line 64: {"applicationVersion":"1.0.0+e45c80b", ...}   -- no "demoMode" key: that commit predates Task 35.7
- Line 66: 4. Return to the current revision.
- Line 70: {"applicationVersion":"1.0.0+aac9abe968d1...", ..., "demoMode":true}
- Line 85: 3. Run the migrate job by hand
- Line 96: 4. Run the seed job on the demo
- Line 107: 5. Rotate the two database role passwords
- Line 119: 6. Rotate the demo password
- Line 155: 7. Read logs and find a trace by correlation ID
- Line 170: 8. When readiness fails
- Line 195: 9. Add and remove a temporary administrator IP
- Line 202: ... do the work ...
- Line 208: 10. Who may approve production deployments

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Runbook

Commands that work when followed literally. Each section was either run for real today (2026-10-11, against
the local production stack — no Azure subscription is provisioned, see `docs/architecture/deployment.md`) or
is marked **not executed**, with the reason, rather than left to guesswork.

**End of Month 3 (2026-10-26).** Cloud deployment is still not set up, so every "Deploy" and Azure step below
remains **not executed**; Month 3 added no deployment step. The Month 3 checks were run against a freshly seeded
local instance instead (`docs/operations/month3-local-verification.md`).

## 1. Deploy

**Not executed.** `deploy.yml` is not written in this slice: every step depends on a resource (registry,
jobs, container app, OIDC trust) that does not exist yet (Tasks 35.1–35.5). The commands below are the design
from `docs/architecture/deployment.md`'s six-step flow, to be proven true once that work lands.

**Normal path** — triggered automatically after CI goes green on a push to `main`:
1. `deploy.yml` runs in the `production` GitHub environment, authenticating to Azure via OIDC (no stored secret).
2. Build and push the image, tagged with the commit SHA.
3. Update and start the `migrate` job with that image and the owner connection; wait for success.
4. Update the container app's image; wait for the new revision to become ready.
5. Run `smoke/` (Task 35.8) against the new revision.
6. On smoke failure, activate the previous revision and fail the run.

```

The excerpt stops at line 24; use the original file for the remaining 190 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
