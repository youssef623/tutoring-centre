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

## Addendum: 2026-10-09 — `SaveChangesAsync` returns a `Result`

Month 1 left a known gap: `SaveChangesAsync` returned `Task`, so a failure only the database could detect at
write time — a lost uniqueness race, a stale optimistic-concurrency version — had no way to come back as
anything but an exception (recorded against the centre-slug race test, Day 9). Task 25.3 changes the port to
`Task<Result> SaveChangesAsync(CancellationToken ct)`. The dispatcher now inspects that result exactly like a
handler's: on failure it rolls back and returns `Result<TResponse>.Failure` carrying the save's error (pipeline
case C14, `docs/notes/pipeline-cases.md`); it commits only once the save has actually succeeded. The real
adapter (`UnitOfWork`, Infrastructure) translates the two expected database failure families — unique
violations and concurrency conflicts — into that `Result` (Task 25.4); every other database failure still
throws and still ends in rollback and an unhandled exception, unchanged from the original decision above. No
EF Core or Npgsql type crosses into Application: the port still speaks only `Result`.
