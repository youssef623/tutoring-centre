# `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): A settable stand-in for `ICentreTimeZoneProvider`; starts at Cairo, the zone every seeded centre uses.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 521 bytes. Source hash: `4a98f00d91795874bd771d6cb7fffa4a28e3c09115cfcb6eae60dcef1cd38533`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Contracts;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs#L6): `public sealed class FakeCentreTimeZoneProvider : ICentreTimeZoneProvider`
- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs#L10): `public Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct) => Task.FromResult(Zone);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:153](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L153)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:125](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L125)
- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:114](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L114)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:144](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L144)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:21](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Contracts;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A settable stand-in for <see cref="ICentreTimeZoneProvider"/>; starts at Cairo, the zone every seeded centre uses.</summary>
public sealed class FakeCentreTimeZoneProvider : ICentreTimeZoneProvider
{
    public TimeZoneInfo Zone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct) => Task.FromResult(Zone);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
