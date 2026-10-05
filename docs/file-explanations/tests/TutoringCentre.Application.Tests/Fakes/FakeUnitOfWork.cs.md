# tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs

## Purpose

In-memory `IUnitOfWork` that records every call in order.

## Where It Fits

Application.Tests/Fakes. Used by `DispatcherTests`; implements `Application/Common/Ports/IUnitOfWork`.

## Walkthrough

`Calls` list of strings. `BeginAsync` adds `Begin(ro)` or `Begin(rw)`; `SaveChangesAsync` adds `Save` and then throws `InvalidOperationException` if `ThrowOnSave` is true (so the call is recorded before the failure, matching case C5); `CommitAsync` adds `Commit`; `RollbackAsync` adds `Rollback`. Use one instance per test (state accumulates). CA1716 suppressed for the `readOnly` parameter with justification.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Recording fake.

#### How it works here

Whole class.

#### Why it matters here

Behaviour verification.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs`](../../../src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs.md)
- [`tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`](../Cqrs/DispatcherTests.cs.md)
