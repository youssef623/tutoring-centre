# src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs

## Purpose
A thin port over the persistence unit of work: a DbContext plus a database transaction. Its doc (lines 5–8) says only the dispatcher calls it, with one transaction and at most one `SaveChanges` per command, and handlers never touch it.

## Where it fits
Application/Common/Ports.
- **Implemented by:** `Infrastructure/Persistence/UnitOfWork.cs`.
- **Registered:** scoped, `Infrastructure/DependencyInjection.cs:56`.
- **Faked by:** `tests/.../Fakes/FakeUnitOfWork.cs`.
- **Consumer:** `Dispatcher`.

## Walkthrough
- **Lines 12–16:** `Task BeginAsync(bool readOnly, CancellationToken ct)`, where `readOnly` is true for queries. It carries `[SuppressMessage("Naming","CA1716")]` because `ReadOnly` is a VB keyword; justified as a C#-only solution.
- **Line 19:** `SaveChangesAsync`, for commands only.
- **Line 22:** `CommitAsync`.
- **Line 25:** `RollbackAsync`.

## Concepts used
- **Unit of Work pattern:** groups all changes of a business operation into one atomic commit.
- **Port / Dependency Inversion:** the Dispatcher controls transactions without referencing EF Core, which `DependencyRuleTests` forbids in Application.

## Data and control flow
Dispatcher → Begin → (handler) → Save → Commit / Rollback.

## Configuration and environment
None.

## Gotchas and issues
- The "handlers never call it" rule is convention only. A handler could inject `IUnitOfWork`, and nothing would stop it.

## Related files
- [UnitOfWork.cs](../../../TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
- [Dispatcher.cs](../Cqrs/Dispatcher.cs.md)
- [FakeUnitOfWork.cs](../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md)
