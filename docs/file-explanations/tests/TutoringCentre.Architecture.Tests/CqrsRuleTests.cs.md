# tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs

## Purpose

Reflection-based architecture rules for CQRS: exactly one handler per request, handlers sealed and non-public, query handlers never depend on repositories.

## Where It Fits

Architecture.Tests. Inspects the Application assembly with plain reflection (not NetArchTest). Mirrors how `Dispatcher` resolves handlers.

## Walkthrough

`ApplicationTypes` (1) is `SourceAssemblies.Application.GetTypes()`.
- `EveryCommandAndQuery_HasExactlyOneHandler` (10-25): requests = concrete classes implementing `ICommand<>` or `IQuery<>`; for every handler (a concrete class implementing `ICommandHandler<,>` or `IQueryHandler<,>`) `HandledRequests` reads the first generic argument of each handler interface; groups and counts per request; any request with count != 1 is a violation (`"Type: n handlers"`). Non-empty guard on `requests`.
- `Handlers_AreSealedAndNotPublic` (27-38): `!IsSealed || IsPublic || IsNestedPublic` is a violation.
- `QueryHandlers_DoNotDependOnRepositories` (40-54): for query handlers, any constructor parameter whose type is a repository interface is a violation (queries must read through read services, not repositories).
Helpers (lines 56-70): `ConcreteClasses`, `Handlers`, `HandledRequests`, `Implements(type, openGenericInterface)`.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

One request, one handler.

#### How it works here

`EveryCommandAndQuery_HasExactlyOneHandler`.

#### Why it matters here

`Dispatcher` resolves a single handler per request type from DI; zero handlers would fail at runtime, two would make the choice ambiguous. The test moves that failure to build time.

### Architecture rules beyond dependencies

#### What it means

Beyond 'who references whom', tests can assert design conventions with reflection and IL scanning: every command has exactly one handler, handlers are sealed and internal, repositories live in the right project, endpoints do not touch Infrastructure.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies](../../../PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies).)

#### Where it appears in this file

Sealed and non-public handlers; no repositories in queries.

#### How it works here

`Handlers_AreSealedAndNotPublic` and `QueryHandlers_DoNotDependOnRepositories`.

#### Why it matters here

Handlers are an implementation detail reachable only through the Dispatcher, and the read side is kept separate from the write side.

### Generics and constraints

#### What it means

Generics let one definition work for many types (`Result<T>`, `ICommandHandler<TCommand,TResponse>`). Constraints (`where TCommand : ICommand<TResponse>`) restrict which types are legal so the compiler can check them; the `in` modifier (contravariance) lets a handler of a base type satisfy a derived one.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Open generic interface matching.

#### How it works here

`HandledRequests` and `Implements` compare `GetGenericTypeDefinition()` with `typeof(ICommandHandler<,>)` and the other open generic interfaces.

#### Why it matters here

The only way to ask 'does this type implement some closed version of this generic interface' with reflection.

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

Reflection instead of NetArchTest.

#### How it works here

Whole file.

#### Why it matters here

Some rules (counting handlers per request) are easier to express directly in reflection than in NetArchTest's fluent API.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs`](SourceAssemblies.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs`](ArchitectureSupport.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
