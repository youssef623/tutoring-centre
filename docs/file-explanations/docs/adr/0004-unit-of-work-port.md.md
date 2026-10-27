# `docs/adr/0004-unit-of-work-port.md`

## Purpose

Documentation covering ADR 0004: A thin unit-of-work port called only by the dispatcher; Context; Decision; Alternatives considered; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0004-unit-of-work-port.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 3,311 bytes. Source hash: `f74b6ce3808b71b4b6a5ea02feaeec9deeb244193790c9fc04161697c69f47e5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0004: A thin unit-of-work port called only by the dispatcher
- Line 6: Context
- Line 10: Decision
- Line 14: Alternatives considered
- Line 20: Consequences
- Line 27: Addendum: 2026-10-09 — `SaveChangesAsync` returns a `Result`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0004: A thin unit-of-work port called only by the dispatcher

- **Status:** Accepted
- **Date:** 2026-10-04

## Context

The Application layer must not reference Entity Framework Core (the dependency rule is enforced by tests), yet a command must save its changes atomically and a query must be unable to write. `DbContext` already is a unit of work, but Application cannot see it.

## Decision

Application defines `IUnitOfWork` (`src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs`) with four operations — `BeginAsync(readOnly)`, `SaveChangesAsync`, `CommitAsync`, `RollbackAsync` — implemented in Infrastructure over `AppDbContext` and a database transaction. Only the dispatcher calls it. Handlers and repositories never save: repositories track changes, and the dispatcher saves once and commits. A read-only transaction issues `SET TRANSACTION READ ONLY`, so PostgreSQL itself rejects writes made inside a query.

## Alternatives considered

1. **Handlers call `SaveChanges` themselves.** Easy, but a handler can save twice, forget to, or save inside a failed path; atomicity depends on every author.
2. **A full repository-level unit of work (repositories own persistence).** More indirection and a bigger port for no gain over `DbContext`'s change tracking.
3. **A generic repository exposing `IQueryable`.** Leaks EF into Application and encourages query logic in handlers; repositories here are per-aggregate with methods named by use, and reads go through read services returning DTOs.

## Consequences

- One atomic commit per command; failures and exceptions roll back, proven against real PostgreSQL by `tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs`.
- Queries are read-only by construction, proven by `tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs` (SQLSTATE 25006).
- Handlers are unit-testable with a fake unit of work and fake repositories (`tests/TutoringCentre.Application.Tests/Fakes/`).
```

The excerpt stops at line 24; use the original file for the remaining 14 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
