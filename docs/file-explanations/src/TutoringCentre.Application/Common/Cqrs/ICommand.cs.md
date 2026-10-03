# src/TutoringCentre.Application/Common/Cqrs/ICommand.cs

## Purpose

Marker interface for commands, carrying the response type as a generic parameter.

## Where It Fits

Application/Common/Cqrs. Implemented by `CreateCentreCommand` (and test commands); constrains `Dispatcher.SendAsync<TCommand,TResponse>`.

## Walkthrough

`public interface ICommand<TResponse>;` (empty). The type parameter lets the compiler link a command to its response so `ICommandHandler<TCommand,TResponse>` and `SendAsync` agree. Doc: handled by exactly one handler; use `Unit` when nothing is returned.

## Concepts Used

### Generics and constraints

#### What it means

Generics let one definition work for many types (`Result<T>`, `ICommandHandler<TCommand,TResponse>`). Constraints (`where TCommand : ICommand<TResponse>`) restrict which types are legal so the compiler can check them; the `in` modifier (contravariance) lets a handler of a base type satisfy a derived one.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

`ICommand<TResponse>`.

#### How it works here

Interface declaration.

#### Why it matters here

Compile-time pairing of command and response type.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Nothing enforces 'exactly one handler'; a second registration would make `GetRequiredService` return the last one.

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs`](ICommandHandler.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](Dispatcher.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs`](../../Centres/Commands/CreateCentre/CreateCentreCommand.cs.md)
