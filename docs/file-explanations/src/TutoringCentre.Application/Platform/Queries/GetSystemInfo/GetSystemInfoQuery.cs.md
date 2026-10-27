# `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Asks for the application version and database migration status.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 6 lines and 269 bytes. Source hash: `dbfe5903907d9e7aa7972e1b67b126102358eee67d31da2c69822de3fe78968a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs#L6): `public sealed record GetSystemInfoQuery : IQuery<SystemInfoDto>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs:26](../../../../../../../src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs#L26)
- [src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs:14](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs#L14)
- [tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs:15](../../../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs:13](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Platform.Queries.GetSystemInfo;

/// <summary>Asks for the application version and database migration status.</summary>
public sealed record GetSystemInfoQuery : IQuery<SystemInfoDto>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
