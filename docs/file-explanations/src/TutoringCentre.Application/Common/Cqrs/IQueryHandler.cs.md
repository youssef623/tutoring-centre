# src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs

## Purpose

Contract for a class that answers one query inside a read-only transaction.

## Where It Fits

Application/Common/Cqrs. Implemented by `GetSystemInfoHandler`.

## Walkthrough

`IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>` with `HandleAsync`. Doc: runs inside a read-only transaction and never saves (dispatcher, 'Day 7').

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Query handler contract.

#### How it works here

Interface.

#### Why it matters here

Separate pipeline from commands.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

A query handler that writes is rejected by PostgreSQL (`SET TRANSACTION READ ONLY`, SQLSTATE 25006) - proven by `ReadOnlyQueryTests`.

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/IQuery.cs`](IQuery.cs.md)
- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](../../Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](Dispatcher.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs`](../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs.md)
