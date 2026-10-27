# `docs/adr/0006-tenant-isolation.md`

## Purpose

Documentation covering ADR 0006: Tenant isolation as defense in depth; Context; Alternatives considered; Decision; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0006-tenant-isolation.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 6,299 bytes. Source hash: `246a3990525f4e2fde7b0167bf15e19fe829015c962e35be05d3ee5c20ca9b2c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0006: Tenant isolation as defense in depth
- Line 7: Context
- Line 19: Alternatives considered
- Line 42: Decision
- Line 77: Consequences

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0006: Tenant isolation as defense in depth

- **Status:** Accepted — all four layers (0, 1, 2, 3 and 4) implemented and proven (Days 19–22), tracked in
  `docs/architecture/tenancy.md`.
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
```

The excerpt stops at line 24; use the original file for the remaining 61 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
