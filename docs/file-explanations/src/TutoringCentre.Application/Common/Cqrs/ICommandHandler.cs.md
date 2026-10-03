# src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs

## Purpose
The contract for executing one command: `Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken)`. It always returns a `Result`, because business failures are values, not exceptions (line 5).

## Where it fits
Application CQRS contracts. Found by `HandlerRegistration.IsHandlerInterface` and resolved by `Dispatcher.SendAsync` (line 47). Implemented in tests by `TestCommandHandler`.

## Walkthrough
- **Lines 6–7:** `public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>`.
  - `in` makes `TCommand` contravariant: a handler for a base command type can serve a derived one.
- **Line 9:** the single async method.

## Concepts used
- **Generic contravariance (`in`).**
- **Result-returning handlers.**
- **Cancellation tokens.**

## Data and control flow
Dispatcher → `HandleAsync` → `Result<TResponse>` → Dispatcher decides save/commit vs rollback.

## Configuration and environment
None.

## Gotchas and issues
- Per `IUnitOfWork.cs:7`, handlers must **never** call the unit of work or `SaveChanges`; the Dispatcher owns that. Nothing enforces this rule.

## Related files
- [ICommand.cs](ICommand.cs.md)
- [Dispatcher.cs](Dispatcher.cs.md)
- [TestRequests.cs](../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs.md)
