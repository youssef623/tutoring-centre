# src/TutoringCentre.Application/Common/Cqrs/IQuery.cs

## Purpose

Marker interface for read-only requests.

## Where It Fits

Application/Common/Cqrs. Implemented by `GetSystemInfoQuery`; constrains `Dispatcher.QueryAsync`.

## Walkthrough

`public interface IQuery<TResponse>;`. The command/query split is what lets the dispatcher open a read-only transaction for queries and never call `SaveChanges`.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Query marker.

#### How it works here

Interface.

#### Why it matters here

Type-level separation of reads and writes.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs`](IQueryHandler.cs.md)
- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs`](../../Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](Dispatcher.cs.md)
