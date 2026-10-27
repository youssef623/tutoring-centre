# `src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs`

## Purpose

The pure generator evaluates each local date separately, applies explicit skipped/repeated DST policy, converts to UTC, and adds duration as elapsed minutes. It accepts zone and range explicitly and reads no clock or repository. Repeated UTC additions would break its wall-clock recurrence contract across offset changes.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 75 lines and 4,055 bytes. Source hash: `a42b9eb0921cb2ecddc355aa30d8d2147649414a47808a5c3458ab83084a4592`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs#L10): `public static class SessionGenerator`
- [Line 17](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs#L17): `public static IReadOnlyList<ClassSession> Generate(ScheduleRule rule, Guid teacherId, TimeZoneInfo zone, DateOnly from, DateOnly to)`
- [Line 50](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs#L50): `private static DateTimeOffset ToUtc(DateTime local, TimeZoneInfo zone)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:67](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L67)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:54](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L54)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs:32](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L32)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs:91](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L91)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs:40](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L40)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Academics.Scheduling;

/// <summary>
/// Turns a local, recurring <see cref="ScheduleRule"/> into dated <see cref="ClassSession"/>s with UTC instants.
/// A domain service: the logic belongs to neither the rule nor a session alone. Pure: every input is a parameter
/// — no clock, no repository, no machine time zone — so the same inputs always give the same dates and instants.
/// What it does not decide: "today" (the caller passes the range), which dates already have sessions (the caller
/// filters on rule and local date), and overlap with other sessions (the database's exclusion constraints).
/// </summary>
public static class SessionGenerator
{
    /// <summary>
    /// One session for each date from <paramref name="from"/> to <paramref name="to"/> (both inclusive) on which
    /// the rule occurs, in date order. Empty when <paramref name="to"/> is before <paramref name="from"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="teacherId"/> is empty — the caller passes the group's teacher, which is never empty.</exception>
    public static IReadOnlyList<ClassSession> Generate(ScheduleRule rule, Guid teacherId, TimeZoneInfo zone, DateOnly from, DateOnly to)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(zone);
        if (teacherId == Guid.Empty)
        {
            throw new ArgumentException("A teacher ID is required.", nameof(teacherId));
        }

        var sessions = new List<ClassSession>();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            if (!rule.OccursOn(date))
            {
                continue;
            }

            // Each date's instant is computed from that date's own wall-clock time. Never take the first instant
            // and add 7 days: after a clock change every later session would be an hour off.
            var localStart = date.ToDateTime(rule.StartTime, DateTimeKind.Unspecified);
            var startsAt = ToUtc(localStart, zone);

            // Elapsed time, not wall-clock arithmetic: a 120-minute session is 120 minutes long even when the
            // clocks go back in the middle of it.
            var endsAt = startsAt.AddMinutes(rule.DurationMinutes);

            sessions.Add(ClassSession.Schedule(rule, teacherId, date, startsAt, endsAt));
        }

        return sessions;
    }

    /// <summary>Converts a wall-clock time in <paramref name="zone"/> to the UTC instant it means. The three cases are the only place transition-hour rules live.</summary>
    private static DateTimeOffset ToUtc(DateTime local, TimeZoneInfo zone)
    {
        // Skipped (spring forward): this wall-clock time never happens. Move it forward by the size of the gap.
        // Cairo, 30 April 2027: 00:00 jumps to 01:00, so 00:30 becomes 01:30 UTC+3 (22:30 UTC on 29 April).
        // The offsets a day either side are outside the gap: the one in force before it and the one after.
        if (zone.IsInvalidTime(local))
        {
            var offsetBefore = zone.GetUtcOffset(local.AddDays(-1));
            var offsetAfter = zone.GetUtcOffset(local.AddDays(1));
            var shifted = local + (offsetAfter - offsetBefore);
            return new DateTimeOffset(shifted, offsetAfter).ToUniversalTime();
        }

        // Repeated (autumn back): this wall-clock time happens twice. Take the first occurrence, which is the one
        // with the larger offset. Cairo, 28 October 2027: 24:00 goes back to 23:00, so 23:30 is taken at UTC+3
        // (20:30 UTC), not at UTC+2 an hour later.
        if (zone.IsAmbiguousTime(local))
        {
            var firstOccurrenceOffset = zone.GetAmbiguousTimeOffsets(local).Max();
            return new DateTimeOffset(local, firstOccurrenceOffset).ToUniversalTime();
        }

        // Normal: the zone's offset on that date (UTC+2 in Cairo's winter, UTC+3 in its summer).
        return new DateTimeOffset(local, zone.GetUtcOffset(local)).ToUniversalTime();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
