# `src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): Errors the schedule handlers raise themselves; the entities raise their own. Double-booking has no entry here: no handler checks for overlaps, and the conflict comes from the exclusion constraints through the persistence error translator.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 1,269 bytes. Source hash: `0f8d26631352719e662fa56f2e260b52a51030876f2d914986d8c1e6d752c479`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs#L10): `internal static class ScheduleErrors`
- [Line 13](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs#L13): `public static Error RuleNotFound() => Error.NotFound("schedule.rule_not_found", "This schedule rule could not be found.");`
- [Line 16](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs#L16): `public static Error SessionNotFound() => Error.NotFound("schedule.session_not_found", "This session could not be found.");`
- [Line 18](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs#L18): `public static Error GroupArchived() => Error.Rule("group.archived", "This group is archived and cannot be scheduled.");`
- [Line 21](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleErrors.cs#L21): `public static Error RangeInvalid(string field, string message) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:43](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L43)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:33](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Errors the schedule handlers raise themselves; the entities raise their own. Double-booking has no entry here:
/// no handler checks for overlaps, and the conflict comes from the exclusion constraints through the
/// persistence error translator.
/// </summary>
internal static class ScheduleErrors
{
    /// <summary>The one answer for an unknown ID and another centre's rule.</summary>
    public static Error RuleNotFound() => Error.NotFound("schedule.rule_not_found", "This schedule rule could not be found.");

    /// <summary>The one answer for an unknown ID and another centre's session.</summary>
    public static Error SessionNotFound() => Error.NotFound("schedule.session_not_found", "This session could not be found.");

    public static Error GroupArchived() => Error.Rule("group.archived", "This group is archived and cannot be scheduled.");

    /// <summary>A date outside the range the command accepts, naming the offending field.</summary>
    public static Error RangeInvalid(string field, string message) =>
        new("schedule.range_invalid", message, ErrorKind.Rule, new Dictionary<string, string[]> { [field] = [message] });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
