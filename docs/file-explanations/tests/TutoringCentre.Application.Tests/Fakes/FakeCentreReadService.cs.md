# `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for centre settings reads.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 478 bytes. Source hash: `823f4409318efdbf68c0e27552c11cd9b71f8578b7b4d7d6883c399b30942d97`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs#L6): `public sealed class FakeCentreReadService : ICentreReadService`
- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs#L10): `public Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:98](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L98)
- [tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for centre settings reads.</summary>
public sealed class FakeCentreReadService : ICentreReadService
{
    public Dictionary<Guid, CentreSettingsDto> SettingsByCentreId { get; } = [];

    public Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct) =>
        Task.FromResult(SettingsByCentreId.GetValueOrDefault(centreId));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
