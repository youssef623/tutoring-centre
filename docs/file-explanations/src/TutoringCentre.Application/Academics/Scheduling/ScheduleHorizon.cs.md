# `src/TutoringCentre.Application/Academics/Scheduling/ScheduleHorizon.cs`

## Purpose

Source summary (intent, not independent execution proof): How far ahead, in calendar days of the centre's zone, schedules are generated and accepted.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleHorizon.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 658 bytes. Source hash: `bcd1a6637ee1e913abb8490dd2d6789df5297fdda0253baf67588297515f5943`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleHorizon.cs#L4): `public static class ScheduleHorizon`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:46](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L46)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:21](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L21)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:48](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L48)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:31](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L31)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>How far ahead, in calendar days of the centre's zone, schedules are generated and accepted.</summary>
public static class ScheduleHorizon
{
    /// <summary>Adding a rule generates its sessions from today through today plus <c>Days - 1</c>: four weeks.</summary>
    public const int Days = 28;

    /// <summary>A new rule may take effect at most this many days after today.</summary>
    public const int MaxEffectiveFromDaysAhead = 366;

    /// <summary>Extending the schedule may reach at most this many days after today.</summary>
    public const int MaxExtendDaysAhead = 92;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
