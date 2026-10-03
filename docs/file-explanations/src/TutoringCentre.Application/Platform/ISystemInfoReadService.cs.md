# src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs

## Purpose

Read-side port that returns database schema status as plain data, plus the `SchemaStatus` record.

## Where It Fits

Application/Platform. Implemented by `Infrastructure/ReadServices/SystemInfoReadService`; used by `GetSystemInfoHandler`.

## Walkthrough

`Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken)` and `record SchemaStatus(string? LatestAppliedMigration, int PendingMigrationCount)`. Doc: 'Never exposes EF types or domain entities' - the handler never learns EF migration APIs exist.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Read service (query side).

#### How it works here

Interface.

#### Why it matters here

Reads skip the repository/entity path.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs`](../../TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs.md)
- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](Queries/GetSystemInfo/GetSystemInfoHandler.cs.md)
