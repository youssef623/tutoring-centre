# `src/TutoringCentre.Application/Centres/ICentreReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port for a centre's own settings. No list or search: a centre is always read by its own id, never browsed.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Centres/ICentreReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 679 bytes. Source hash: `9cde18fbc43f332430a98c5841acbd1c0b451e179a47de3dde9bac73a4b87bfc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Application/Centres/ICentreReadService.cs#L6): `public interface ICentreReadService`
- [Line 13](../../../../../src/TutoringCentre.Application/Centres/ICentreReadService.cs#L13): `public sealed record CentreSettingsDto(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale, uint Version);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs:9](../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:125](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L125)
- [src/TutoringCentre.Infrastructure/ReadServices/CentreReadService.cs:15](../../../../../src/TutoringCentre.Infrastructure/ReadServices/CentreReadService.cs#L15)
- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:98](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L98)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs:6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres;

/// <summary>Read-side port for a centre's own settings. No list or search: a centre is always read by its own id, never browsed.</summary>
public interface ICentreReadService
{
    /// <summary>One centre's settings by id, or null when no such centre exists.</summary>
    Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct);
}

/// <summary>The contract's settings shape, including the version a later update must send back.</summary>
public sealed record CentreSettingsDto(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale, uint Version);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
