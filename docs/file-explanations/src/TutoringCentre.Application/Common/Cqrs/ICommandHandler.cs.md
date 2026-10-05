# src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs

## Purpose

Contract for a class that executes one command and returns a `Result`.

## Where It Fits

Application/Common/Cqrs. Implemented by `CreateCentreHandler`; resolved by `Dispatcher` via `GetRequiredService<ICommandHandler<TCommand,TResponse>>()`; registered by `HandlerRegistration`.

## Walkthrough

`ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>` with `Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)`. `in` makes `TCommand` contravariant (a handler for a base command type could serve derived types - not used today). Returning `Result` means business failures are values; throwing means a bug/fault.

## Concepts Used

### Generics and constraints

#### What it means

Generics let one definition work for many types (`Result<T>`, `ICommandHandler<TCommand,TResponse>`). Constraints (`where TCommand : ICommand<TResponse>`) restrict which types are legal so the compiler can check them; the `in` modifier (contravariance) lets a handler of a base type satisfy a derived one.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Variance and constraint.

#### How it works here

`in TCommand`, `where TCommand : ICommand<TResponse>`.

#### Why it matters here

Compiler-checked pairing.

### async/await and cancellation

#### What it means

`async`/`await` lets a method wait for I/O (database, network) without blocking a thread: the method returns a `Task`, and execution resumes after the awaited operation completes. A `CancellationToken` is a cooperative signal (for example, the HTTP request was aborted) passed down so work can stop early.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

`Task<...>` + `CancellationToken`.

#### How it works here

`HandleAsync` signature.

#### Why it matters here

Database work awaits; cancellation flows from the HTTP request to EF.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

A handler must not call SaveChanges/Commit (dispatcher owns it).

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/ICommand.cs`](ICommand.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](Dispatcher.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
