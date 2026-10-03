# tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs

## Purpose
An in-memory `IUnitOfWork` that records every call in order (`Begin(rw)`, `Begin(ro)`, `Save`, `Commit`, `Rollback`). It can optionally throw on save. Use one instance per test (line 6).

## Where it fits
Application.Tests. It implements the `IUnitOfWork` port and is used by `DispatcherTests` and the `TestRequests` handlers.

## Walkthrough
- **Line 9:** `List<string> Calls`.
- **Line 12:** `ThrowOnSave`, used for case C5.
- **Lines 14–22:** `BeginAsync` records `Begin(ro)` or `Begin(rw)`. It carries the same CA1716 suppression as the port.
- **Lines 24–33:** `SaveChangesAsync` records `"Save"`, then throws if `ThrowOnSave`.
- **Lines 35–45:** `CommitAsync` and `RollbackAsync` record their names.

## Concepts used
- **Fake / spy test double.**
- **Ports enable fakes:** because the Dispatcher depends on an interface, no database is needed.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **No state machine.** It doesn't model one, so it won't catch protocol errors that the real `UnitOfWork` would (e.g. a commit without a begin).

## Related files
- [IUnitOfWork.cs](../../../src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs.md)
- [UnitOfWork.cs](../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
- [DispatcherTests.cs](../Cqrs/DispatcherTests.cs.md)
