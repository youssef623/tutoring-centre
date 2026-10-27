# `src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Never exposes EF types or domain entities.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 442 bytes. Source hash: `b85572f52ef52d50537f30c2fa4097c77070d07e75c3df9ce6542e81a4edd4c2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs#L4): `public interface ISystemInfoReadService`
- [Line 10](../../../../../src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs#L10): `public sealed record SchemaStatus(string? LatestAppliedMigration, int PendingMigrationCount);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:15](../../../../onboarding-work/extend_backend_purposes.py#L15)
- [src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs:13](../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs#L13)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:154](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L154)
- [src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs:10](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs#L10)
- [src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs:12](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs#L12)
- [tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs:12](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L12)
- [tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs:65](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L65)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Platform;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes EF types or domain entities.</summary>
public interface ISystemInfoReadService
{
    Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct);
}

/// <summary>Database schema status as seen by the read side.</summary>
public sealed record SchemaStatus(string? LatestAppliedMigration, int PendingMigrationCount);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
