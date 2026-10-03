# tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs

## Purpose

End-to-end test of `GetSystemInfoQuery` against a migrated database.

## Where It Fits

Infrastructure.Tests/Platform.

## Walkthrough

`QueryAsAsync<GetSystemInfoQuery,SystemInfoDto>(new AnonymousActor(), new GetSystemInfoQuery())`; asserts success, `LatestMigration` ends with `_InitialPlatform`, `DatabaseUpToDate` true, version non-blank. Uses an anonymous actor, confirming the query needs no authorization.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Real read service.

#### How it works here

Dispatch through DI.

#### Why it matters here

Verifies migration-history reading.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs.md)
- [`src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs`](../../../src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs.md)
