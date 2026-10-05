# ADR 0003: Hand-written CQRS dispatcher

- **Status:** Accepted
- **Date:** 2026-10-04

## Context

Every use case needs the same cross-cutting steps in a fixed order: validate the request, open the right kind of transaction, run the handler, commit or roll back, and log the outcome once. From Month 2 two more steps join them — tenant resolution and permission checks — and they must be impossible to forget on any use case. Handlers are invoked from several clients (HTTP, the seed CLI, later background jobs and an AI assistant), so the steps cannot live in HTTP middleware or endpoints.

## Decision

A small hand-written `Dispatcher` in the Application layer with two explicit pipelines:

- Commands: validate → begin read-write transaction → handler → on failure result roll back, otherwise save once and commit; an exception rolls back and is rethrown to the global exception handler.
- Queries: validate → begin read-only transaction → handler → commit; never saves.

Handlers are resolved by closed generic type (`ICommandHandler<TCommand, TResponse>` / `IQueryHandler<TQuery, TResponse>`); validators are discovered by assembly scanning; handlers return `Result<T>`. The tenant and permission steps will be added between validation and `BeginAsync`.

## Alternatives considered

1. **MediatR with pipeline behaviours.** Mature, but its behaviours are implicit and its licensing changed; the pipeline is the core of this project and worth owning and learning.
2. **Decorators around each handler.** Explicit, but each decorator must be registered per handler and ordering becomes configuration instead of code.
3. **Calling services directly from endpoints.** No shared pipeline; every endpoint would repeat transactions, validation and logging.

## Consequences

- About 100 lines of code to own and test — nine pipeline cases are covered by unit tests (`tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`), proven by deliberate breaks.
- Call sites state their generic arguments explicitly (`SendAsync<TCommand, TResponse>` / `QueryAsync<TQuery, TResponse>`); there is no reflection magic at call time.
- The pipeline order is readable in one file (`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`), and tenancy/permission steps have a defined insertion point.
- Queries physically cannot write: they run in a read-only transaction enforced by PostgreSQL (`SET TRANSACTION READ ONLY`), proven by `tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs`.
