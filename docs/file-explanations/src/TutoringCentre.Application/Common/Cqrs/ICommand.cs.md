# src/TutoringCentre.Application/Common/Cqrs/ICommand.cs

## Purpose
`public interface ICommand<TResponse>;` is the marker for a request that **changes state** and produces `TResponse` on success. Use `Unit` when there is nothing to return. Each command is handled by exactly one handler (lines 3–6).

## Where it fits
Application CQRS contracts. It constrains `ICommandHandler<TCommand,TResponse>` and `Dispatcher.SendAsync`. The test `TestCommand : ICommand<string>` implements it (`TestRequests.cs:26`).

## Walkthrough
- **Line 7:** an empty generic interface. The type parameter exists so the compiler links a command to its response type.

## Concepts used
- **Marker interface with a phantom type parameter:** the parameter is never used in members, but it gives compile-time typing between request and response.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Not covariant (`out` isn't declared). That's irrelevant for a marker.

## Related files
- [ICommandHandler.cs](ICommandHandler.cs.md)
- [Unit.cs](Unit.cs.md)
- [Dispatcher.cs](Dispatcher.cs.md)
