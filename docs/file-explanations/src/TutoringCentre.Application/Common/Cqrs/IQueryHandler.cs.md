# src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs

## Purpose
The contract for executing one query: `Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken)`. Its doc says it runs inside a read-only transaction and never saves (line 5).

## Where it fits
Application CQRS contracts. Registered by `HandlerRegistration` and resolved by `Dispatcher.QueryAsync` (line 91). The test implementation is `TestQueryHandler`.

## Walkthrough
- **Lines 6–7:** `IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>` (`TQuery` is contravariant).
- **Line 9:** `HandleAsync`.

## Concepts used
Same as `ICommandHandler`.

## Data and control flow
Dispatcher (read-only transaction) → `HandleAsync` → `Result` → commit.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [IQuery.cs](IQuery.cs.md)
- [Dispatcher.cs](Dispatcher.cs.md)
