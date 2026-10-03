# src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs

## Purpose

Read-side implementation that reports applied and pending EF migrations as plain data.

## Where It Fits

Infrastructure/ReadServices, `internal`, scoped. Implements `ISystemInfoReadService`; used by `GetSystemInfoHandler`.

## Walkthrough

`GetSchemaStatusAsync`: `db.Database.GetAppliedMigrationsAsync(ct)` (reads `platform.__ef_migrations_history`) and `GetPendingMigrationsAsync(ct)` (compares the assembly's migrations to the history); returns `new SchemaStatus(applied.LastOrDefault(), pending.Count())`. No entity is loaded and no EF type crosses the port.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Read service.

#### How it works here

Whole class.

#### Why it matters here

Query side without domain entities.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Both calls hit the database; executes inside the read-only transaction opened by the dispatcher.

## Related Files

- [`src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs`](../../TutoringCentre.Application/Platform/ISystemInfoReadService.cs.md)
- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](../../TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs`](../../../tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs.md)
