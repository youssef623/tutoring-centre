# src/TutoringCentre.Application/Common/Cqrs/IQuery.cs

## Purpose
`public interface IQuery<TResponse>;` is the marker for a **read that never changes state**. Each query is handled by exactly one query handler (line 3).

## Where it fits
Application CQRS contracts. It constrains `IQueryHandler` and `Dispatcher.QueryAsync`. The test `TestQuery : IQuery<string>` implements it.

## Walkthrough
- **Line 4:** an empty generic marker interface.

## Concepts used
- **Marker interface with a phantom type parameter.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- "Never changes state" is enforced at runtime: `UnitOfWork.BeginAsync(readOnly: true)` runs `SET TRANSACTION READ ONLY`, and PostgreSQL rejects writes.

## Related files
- [IQueryHandler.cs](IQueryHandler.cs.md)
- [Dispatcher.cs](Dispatcher.cs.md)
- [UnitOfWork.cs](../../../TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
