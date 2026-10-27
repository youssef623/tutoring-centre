# `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Returns version and migration status. No authorization check: the data is not sensitive and is served anonymously. Reads go through the read service; no entities and no EF types.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,919 bytes. Source hash: `a5850731067dfd8d3555d78608aa0b88f69b55bbee8b8b1dc50197ba92233926`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Reflection;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs#L13): `internal sealed class GetSystemInfoHandler(ISystemInfoReadService readService, IDemoModeProvider demoModeProvider)`
- [Line 18](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs#L18): `public async Task<Result<SystemInfoDto>> HandleAsync(GetSystemInfoQuery query, CancellationToken cancellationToken)`
- [Line 31](../../../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs#L31): `private static string ReadApplicationVersion()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs:11](../../../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Platform.Queries.GetSystemInfo;

/// <summary>
/// Returns version and migration status. No authorization check: the data is not sensitive and is served anonymously.
/// Reads go through the read service; no entities and no EF types.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetSystemInfoHandler(ISystemInfoReadService readService, IDemoModeProvider demoModeProvider)
    : IQueryHandler<GetSystemInfoQuery, SystemInfoDto>
{
    private const string UnknownVersion = "unknown";

    public async Task<Result<SystemInfoDto>> HandleAsync(GetSystemInfoQuery query, CancellationToken cancellationToken)
    {
        var status = await readService.GetSchemaStatusAsync(cancellationToken);

        var dto = new SystemInfoDto(
            ReadApplicationVersion(),
            status.LatestAppliedMigration,
            DatabaseUpToDate: status.PendingMigrationCount == 0,
            DemoMode: demoModeProvider.IsEnabled);

        return Result<SystemInfoDto>.Success(dto);
    }

    private static string ReadApplicationVersion()
    {
        var informational = typeof(GetSystemInfoHandler).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        // The SDK appends "+<SourceRevisionId>" on its own once the Dockerfile (Task 34.2) passes
        // -p:SourceRevisionId=<git sha> to `dotnet publish` — kept, not stripped, as of Task 34.2: this is
        // precisely how an operator confirms which commit a running instance is actually serving.
        return string.IsNullOrWhiteSpace(informational) ? UnknownVersion : informational;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
