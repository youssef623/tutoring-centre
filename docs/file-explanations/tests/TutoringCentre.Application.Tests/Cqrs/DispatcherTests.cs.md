# tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs

## Purpose
Unit tests for `Dispatcher`. They implement exactly the nine pipeline cases C1–C9 from `docs/notes/pipeline-cases.md`, by asserting the ordered list of calls that `FakeUnitOfWork` and the test handlers record.

## Where it fits
Application.Tests. It uses `TestRequests.cs` (commands, queries, validators, handlers) and `Fakes/FakeUnitOfWork.cs`. It builds a real MS DI `ServiceProvider`.

## Walkthrough
- **Line 12:** `ValidCommand = new("Nile", "nile")`.
- **Tests:**
  - **Lines 14–26, C1:** success → `["Begin(rw)","Handle","Save","Commit"]`.
  - **Lines 28–41, C2:** `TestCommand("", "nile")` → failure `validation.failed`, kind Validation, `Fields` contains `"name"`, no calls.
  - **Lines 43–53, C3:** `HandlerMode.Fail` → the same `TestErrors.HandlerFailure` (record equality) and `["Begin(rw)","Handle","Rollback"]`.
  - **Lines 55–64, C4:** `HandlerMode.Throw` → `InvalidOperationException`, with `Rollback` recorded.
  - **Lines 66–76, C5:** `ThrowOnSave = true` → `["Begin(rw)","Handle","Save","Rollback"]`.
  - **Lines 78–88, C6:** query → `["Begin(ro)","Handle","Commit"]`, no `Save`.
  - **Lines 90–100, C7:** invalid query → `validation.failed`, no calls.
  - **Lines 102–111, C8:** query handler throws → `["Begin(ro)","Handle","Rollback"]`.
  - **Lines 113–124, C9:** `TestCommand("", "far-too-long")`. The keys are `["name","slug"]`. `name` has 2 messages (`NotEmpty` and `MinimumLength` both fail on ""). `slug` is `["Slug is too long."]`.
- **Lines 126–140, `CreateSut(mode)`:**
  - A singleton `FakeUnitOfWork` and `HandlerBehaviour`.
  - Scoped handlers and validators registered by hand.
  - `new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance)`.
- **Lines 142–149, `Fixture`:** a disposable wrapper that disposes the `ServiceProvider`.

## Concepts used
- **Spy test doubles:** they record calls to verify the protocol.
- **Specification-driven tests.**
- **Real DI container in a unit test:** exercises `GetRequiredService` and `GetServices` resolution.
- **xUnit collection expressions** (`Assert.Equal(["..."], list)`).

## Data and control flow
Test → Dispatcher → validators / FakeUnitOfWork / test handler → recorded calls → assertions.

## Configuration and environment
None.

## Gotchas and issues
- **Resolves from the root provider.** Handlers come from the root `ServiceProvider`, not a scope. Scoped services resolved from the root behave as singletons; harmless here, but it differs from production. `ValidateScopes` is off by default.
- **Missing cases:**
  - commit throws;
  - rollback throws (exception masking);
  - query handler returns a failure;
  - no handler registered;
  - cancellation.
- **Logging unverified.** `NullLogger` means the log output isn't verified.

## Related files
- [Dispatcher.cs](../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
- [TestRequests.cs](TestRequests.cs.md)
- [FakeUnitOfWork.cs](../Fakes/FakeUnitOfWork.cs.md)
- [pipeline-cases.md](../../../docs/notes/pipeline-cases.md.md)
