# tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs

## Purpose

Test-only commands, queries, handlers and validators used to drive `Dispatcher` in isolation.

## Where It Fits

Application.Tests/Cqrs. Used only by `DispatcherTests`. Implements the real `ICommand`, `IQuery`, handler interfaces and FluentValidation `AbstractValidator`.

## Walkthrough

- `enum HandlerMode { Succeed, Fail, Throw }` and `HandlerBehaviour { Mode }` - a mutable switch registered as a singleton so each test chooses what handlers do.
- `TestErrors.HandlerFailure = Error.Rule("test.handler_failed", ...)` - a shared instance so equality assertions work.
- `TestCommand(Name, Slug) : ICommand<string>`, `TestQuery(Name) : IQuery<string>`.
- `TestCommandValidator`: `Name` NotEmpty ('Name is required.') and MinimumLength(3) ('Name is too short.'), `Slug` MaximumLength(5) ('Slug is too long.') - two rules on `Name` so the grouping test can see two messages. `TestQueryValidator`: `Name` NotEmpty.
- `TestCommandHandler`: records `"Handle"` in `FakeUnitOfWork.Calls`, then switches on `Mode` (throw / `Failure` / `Success(command.Name)`). `TestQueryHandler` likewise (throws or succeeds).

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Recording handlers.

#### How it works here

`Calls.Add("Handle")`.

#### Why it matters here

Lets tests assert the order of pipeline steps.

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Real interfaces.

#### How it works here

Handler classes.

#### Why it matters here

Exercises the actual dispatcher generics.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`](DispatcherTests.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs`](../Fakes/FakeUnitOfWork.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
