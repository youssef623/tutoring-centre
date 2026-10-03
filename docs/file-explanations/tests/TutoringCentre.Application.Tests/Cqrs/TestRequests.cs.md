# tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs

## Purpose
Test-only CQRS types used by `DispatcherTests`:
- a command and a query;
- their FluentValidation validators;
- handlers whose behaviour (succeed, fail, throw) is switched per test;
- a shared test error.

## Where it fits
Application.Tests. It implements `ICommand<string>`, `IQuery<string>`, `ICommandHandler`, `IQueryHandler` and FluentValidation's `AbstractValidator`.

## Walkthrough
- **Lines 8–13:** `enum HandlerMode { Succeed, Fail, Throw }`.
- **Lines 16–19:** `HandlerBehaviour { Mode }`, injected as a singleton to steer the handlers.
- **Lines 21–24:** `TestErrors.HandlerFailure = Error.Rule("test.handler_failed", ...)`.
- **Line 26:** `record TestCommand(string Name, string Slug) : ICommand<string>`.
- **Line 28:** `record TestQuery(string Name) : IQuery<string>`.
- **Lines 30–38, `TestCommandValidator`:** Name `NotEmpty` ("Name is required."), Name `MinimumLength(3)` ("Name is too short."), Slug `MaximumLength(5)` ("Slug is too long.").
- **Lines 40–46:** `TestQueryValidator` with Name `NotEmpty`.
- **Lines 48–61, `TestCommandHandler(FakeUnitOfWork, HandlerBehaviour)`:** records `"Handle"` into the *unit of work's* call list, so all calls share one ordered log. Then it throws, fails or returns `Success(command.Name)`.
- **Lines 63–73:** `TestQueryHandler` does the same but has no `Fail` mode; it throws or succeeds.

## Concepts used
- **Configurable test doubles.**
- **Primary-constructor classes.**
- **Switch expressions with `throw`.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Because handlers write into `FakeUnitOfWork.Calls`, the test double depends on the concrete fake. That's deliberate, to get one interleaved sequence.

## Related files
- [DispatcherTests.cs](DispatcherTests.cs.md)
- [FakeUnitOfWork.cs](../Fakes/FakeUnitOfWork.cs.md)
