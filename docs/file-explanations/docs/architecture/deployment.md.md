# `docs/architecture/deployment.md`

## Purpose

Documentation covering Deployment; Topology; The six-step deployment flow; Secrets inventory; The proxy network value. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/deployment.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 185 lines and 11,740 bytes. Source hash: `76e49d456a0a3d2f485ab2ea9ada2574bfe8b8b415c4462916bea8607adb9f52`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Deployment
- Line 21: Topology
- Line 57: The six-step deployment flow
- Line 78: Secrets inventory
- Line 92: The proxy network value
- Line 103: Content-Security-Policy and its one relaxation
- Line 120: Telemetry
- Line 137: then run the Api with Otel__Endpoint=http://localhost:18889 and open http://localhost:18888
- Line 143: The demo banner and demo mode
- Line 155: Running migrate and seed by hand
- Line 168: The deployed smoke test
- Line 179: What remains

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Deployment

How the system is built, proxied, and run, and the pipeline designed to put it on a public URL. Written from
the implementation, with one exception called out up front.

**Status (2026-10-11).** No Azure subscription exists for this project yet. Tasks 35.1–35.5 (provisioning
script, cloud database, GitHub↔Azure OIDC trust, `deploy.yml`, runtime configuration) and the Azure-dependent
parts of 35.6/35.7/35.8/35.9 are therefore not done — not "done against a different target," genuinely not
done. What follows is split accordingly: sections marked **(built)** describe what runs today, locally and in
CI, on the local production stack from Day 34. Sections marked **(designed, not provisioned)** describe the
Azure topology and pipeline the manual specifies, documented so the work that remains is exactly these
sections, done for real, once a subscription exists.

**Status at the end of Month 3 (2026-10-26).** Unchanged: cloud deployment is still not set up, and Month 3 added
none. No Azure (or other cloud) resource exists, `deploy.yml` does not exist, and Month 2's Days 35–36 remain open
(`later.md`). Everything Month 3 built (students, parents, teachers, groups, enrollments, the schedule, and five
migrations ending with `AddSchedule`, which needs the `btree_gist` extension) runs locally, in CI and on the local
production stack only; the Month 3 verification was walked on a freshly seeded local instance
(`docs/operations/month3-local-verification.md`), not on a deployed one.

## Topology

**(built)** — the local production stack, `compose.prod.yaml`:

```

The excerpt stops at line 24; use the original file for the remaining 161 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
