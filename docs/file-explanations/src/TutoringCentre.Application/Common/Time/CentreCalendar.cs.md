# `src/TutoringCentre.Application/Common/Time/CentreCalendar.cs`

## Purpose

Source summary (intent, not independent execution proof): "Today" is not a server date: it is the calendar date in the centre's time zone. At 22:30 UTC on 31 December it is already 1 January in Cairo. Handlers that need a date range ("from today") ask here, with the zone from `TutoringCentre.Application.Centres.Contracts.ICentreTimeZoneProvider`.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Time/CentreCalendar.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 752 bytes. Source hash: `e4dc7b1aae00339d5f883f36366cde322c6d888cd4600d94d0987b18cebe1a76`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Ports;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Common/Time/CentreCalendar.cs#L10): `public static class CentreCalendar`
- [Line 12](../../../../../../src/TutoringCentre.Application/Common/Time/CentreCalendar.cs#L12): `public static DateOnly Today(IClock clock, TimeZoneInfo zone)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:581](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L581)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:42](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L42)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:47](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L47)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:30](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L30)
- [tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs:17](../../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Application.Common.Time;

/// <summary>
/// "Today" is not a server date: it is the calendar date in the centre's time zone. At 22:30 UTC on 31 December
/// it is already 1 January in Cairo. Handlers that need a date range ("from today") ask here, with the zone from
/// <see cref="TutoringCentre.Application.Centres.Contracts.ICentreTimeZoneProvider"/>.
/// </summary>
public static class CentreCalendar
{
    public static DateOnly Today(IClock clock, TimeZoneInfo zone)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(zone);
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.UtcNow, zone).DateTime);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
