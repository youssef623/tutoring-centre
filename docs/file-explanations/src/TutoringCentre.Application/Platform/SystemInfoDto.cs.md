# src/TutoringCentre.Application/Platform/SystemInfoDto.cs

## Purpose

The public response shape of `GET /api/system/info`: application version, latest migration, up-to-date flag.

## Where It Fits

Application/Platform. Produced by `GetSystemInfoHandler`; returned by `PlatformEndpoints`; mirrored in `frontend/README.md` and MSW handlers.

## Walkthrough

`public sealed record SystemInfoDto(string ApplicationVersion, string? LatestMigration, bool DatabaseUpToDate);`. Doc: deliberately excludes environment names, connection details and counts (non-sensitive, anonymous). JSON property names become camelCase (`applicationVersion`, `latestMigration`, `databaseUpToDate`) through ASP.NET's default web JSON settings.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

DTO record.

#### How it works here

Declaration.

#### Why it matters here

A stable, minimal contract that is decoupled from EF.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The latest migration id reveals the migration naming (timestamp + name) to anonymous callers - low risk, by design.

## Related Files

- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](Queries/GetSystemInfo/GetSystemInfoHandler.cs.md)
- [`src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs`](../../TutoringCentre.Api/Endpoints/PlatformEndpoints.cs.md)
- [`frontend/src/test/msw/handlers.ts`](../../../frontend/src/test/msw/handlers.ts.md)
