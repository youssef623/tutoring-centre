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
- The port is deliberately minimal; nested transactions are refused. Needs beyond this (for example row-level-security session settings in Month 2) extend `BeginAsync`, not the handlers.
