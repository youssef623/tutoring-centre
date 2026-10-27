# `docs/adr/0007-code-defined-permissions.md`

## Purpose

Documentation covering ADR 0007: Code-defined permissions; Context; Alternatives considered; Decision; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0007-code-defined-permissions.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 3,092 bytes. Source hash: `37ab3b0de3fc4db07464772532a4d643f1d9c55802e8b562a1aec3a602b6f846`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0007: Code-defined permissions
- Line 6: Context
- Line 13: Alternatives considered
- Line 27: Decision
- Line 35: Consequences

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0007: Code-defined permissions

- **Status:** Accepted
- **Date:** 2026-10-09

## Context

Three staff roles (Owner, Secretary, Teacher) and six permissions exist today, and the full matrix between
them is known in advance — it does not vary per centre, per customer or at runtime. Something still has to
answer "does this role hold this permission" on every tenant-scoped request, and that answer has to be
reviewable, testable and impossible to get subtly wrong one row at a time.

## Alternatives considered

- **Database-defined roles and permissions** (tables an admin edits through a UI, each centre able to define
  its own roles or reassign permissions) is the usual shape once customers genuinely need to customize who
  can do what. It costs real weight today: a schema for roles and role-permission assignments, an admin UI
  and its own authorization (who may edit permissions?), migration or seeding for the default matrix, and an
  audit trail for changes to something security-relevant — all for a matrix that, right now, is the same for
  every centre and changes only when a new feature adds a new permission.
- **Code-defined permissions** (this decision) keeps the matrix as a static map, reviewed in a pull request
  like any other code change, covered by an exhaustive table test that states every cell literally rather
  than deriving it from the map under test. Changing what a role may do means a code change, a test change
  and a deploy — slower than an admin UI, but every change is attributable to a commit and a review, and
```

The excerpt stops at line 24; use the original file for the remaining 22 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
