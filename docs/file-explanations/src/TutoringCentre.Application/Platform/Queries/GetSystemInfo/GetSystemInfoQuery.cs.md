# src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs

## Purpose

The parameterless query asking for application version and migration status.

## Where It Fits

Application/Platform/Queries/GetSystemInfo. Constructed by `PlatformEndpoints`; handled by `GetSystemInfoHandler`.

## Walkthrough

`public sealed record GetSystemInfoQuery : IQuery<SystemInfoDto>;` - no properties, so `new GetSystemInfoQuery()` is all a caller supplies. No validator exists, so `ValidateAsync` finds none and skips validation.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Query message.

#### How it works here

Declaration.

#### Why it matters here

Reads go through the same dispatcher.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](GetSystemInfoHandler.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/IQuery.cs`](../../../Common/Cqrs/IQuery.cs.md)
