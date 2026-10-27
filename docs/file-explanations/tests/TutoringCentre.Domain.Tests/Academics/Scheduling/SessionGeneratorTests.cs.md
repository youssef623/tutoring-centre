# `tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 51.4. Every expected instant is worked out by hand and written as a UTC literal; none is derived with the conversion calls the generator itself makes. Egypt (Africa/Cairo) since 2023: UTC+2 in winter, UTC+3 in summer. Clocks go forward at 00:00 on the last Friday of April and back at 24:00 on the last Thursday of October. For 2027 that is Friday 30 April (00:00 becomes 01:00) and Thursday 28 October (24:00 becomes 23:00, so 23:00-23:59 happens twice). If Egypt's law changes, these dates change and the generator does not. `TimeZoneData_HasEgypts2027Transitions` checks the machine's time-zone data agrees before any failure below is trusted.

## Where it fits

Category: **test**. [Open current file](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 204 lines and 9,922 bytes. Source hash: `6f1f2ba1b327588d8e57ee8b236ef406881867677f164c31c374515b1b7d2689`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L15): `public sealed class SessionGeneratorTests`
- [Line 26](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L26): `public void TimeZoneData_HasEgypts2027Transitions()`
- [Line 36](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L36): `public void Generate_WinterSaturdays_ProducesFourSessionsAtSixteenUtc()`
- [Line 64](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L64): `public void Generate_RangeStartingMidWeek_FirstSessionIsOnTheNextMatchingWeekday()`
- [Line 77](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L77): `public void Generate_EffectiveFromAndToInsideTheRange_BoundTheOutput()`
- [Line 90](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L90): `public void Generate_AcrossSpringForward_WallClockStaysEighteenAndUtcMovesAnHourEarlier()`
- [Line 104](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L104): `public void Generate_AcrossFallBack_WallClockStaysEighteenAndUtcMovesAnHourLater()`
- [Line 118](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L118): `public void Generate_StartInTheSkippedHour_MovesForwardByTheGap()`
- [Line 132](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L132): `public void Generate_StartInTheRepeatedHour_TakesTheFirstOccurrence()`
- [Line 146](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L146): `public void Generate_SessionCrossingFallBack_LastsExactlyItsDurationInElapsedTime()`
- [Line 160](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L160): `public void Generate_JustAfterLocalMidnight_LocalDateIsNotTheUtcDate()`
- [Line 174](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L174): `public void Generate_ToBeforeFrom_ReturnsEmpty()`
- [Line 185](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L185): `public void Generate_Twice_GivesTheSameDatesAndInstantsWithNewIds()`
- [Line 199](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L199): `private static ScheduleRule Rule(DayOfWeek dayOfWeek, TimeOnly startTime, int durationMinutes, DateOnly effectiveFrom) =>`
- [Line 202](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L202): `private static DateTimeOffset Utc(int year, int month, int day, int hour, int minute) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Domain.Tests.Academics.Scheduling;

/// <summary>
/// Task 51.4. Every expected instant is worked out by hand and written as a UTC literal; none is derived with the
/// conversion calls the generator itself makes.
///
/// Egypt (Africa/Cairo) since 2023: UTC+2 in winter, UTC+3 in summer. Clocks go forward at 00:00 on the last
/// Friday of April and back at 24:00 on the last Thursday of October. For 2027 that is Friday 30 April (00:00
/// becomes 01:00) and Thursday 28 October (24:00 becomes 23:00, so 23:00-23:59 happens twice). If Egypt's law
/// changes, these dates change and the generator does not. <see cref="TimeZoneData_HasEgypts2027Transitions"/>
/// checks the machine's time-zone data agrees before any failure below is trusted.
/// </summary>
public sealed class SessionGeneratorTests
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();

    private static readonly DateOnly SpringForward2027 = new(2027, 4, 30);
    private static readonly DateOnly FallBack2027 = new(2027, 10, 28);

    [Fact]
    public void TimeZoneData_HasEgypts2027Transitions()
    {
        Assert.True(Cairo.IsInvalidTime(new DateTime(2027, 4, 30, 0, 30, 0)), "Cairo time-zone data lacks the 30 April 2027 spring-forward gap.");
        Assert.True(Cairo.IsAmbiguousTime(new DateTime(2027, 10, 28, 23, 30, 0)), "Cairo time-zone data lacks the 28 October 2027 repeated hour.");
        Assert.Equal(DayOfWeek.Friday, SpringForward2027.DayOfWeek);
        Assert.Equal(DayOfWeek.Thursday, FallBack2027.DayOfWeek);
    }

    // Case 1
    [Fact]
    public void Generate_WinterSaturdays_ProducesFourSessionsAtSixteenUtc()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 12, 5));

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 5), new DateOnly(2027, 1, 1));

        Assert.Equal(
            [new DateOnly(2026, 12, 5), new DateOnly(2026, 12, 12), new DateOnly(2026, 12, 19), new DateOnly(2026, 12, 26)],
            sessions.Select(session => session.LocalDate));
        Assert.Equal(
            [Utc(2026, 12, 5, 16, 0), Utc(2026, 12, 12, 16, 0), Utc(2026, 12, 19, 16, 0), Utc(2026, 12, 26, 16, 0)],
            sessions.Select(session => session.StartsAt));
        Assert.Equal(
            [Utc(2026, 12, 5, 17, 30), Utc(2026, 12, 12, 17, 30), Utc(2026, 12, 19, 17, 30), Utc(2026, 12, 26, 17, 30)],
            sessions.Select(session => session.EndsAt));
        Assert.All(sessions, session =>
        {
            Assert.Equal(TimeSpan.Zero, session.StartsAt.Offset);
            Assert.Equal(TeacherId, session.TeacherId);
            Assert.Equal(rule.Id, session.ScheduleRuleId);
            Assert.Equal(GroupId, session.GroupId);
            Assert.Equal(CentreId, session.CentreId);
            Assert.Equal(ClassSessionStatus.Scheduled, session.Status);
        });
    }

    // Case 2
    [Fact]
    public void Generate_RangeStartingMidWeek_FirstSessionIsOnTheNextMatchingWeekday()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 11, 1));

        // Wednesday 2 December to Tuesday 15 December.
        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 2), new DateOnly(2026, 12, 15));

        Assert.Equal([new DateOnly(2026, 12, 5), new DateOnly(2026, 12, 12)], sessions.Select(session => session.LocalDate));
        Assert.Equal(Utc(2026, 12, 5, 16, 0), sessions[0].StartsAt);
    }

    // Case 3
    [Fact]
    public void Generate_EffectiveFromAndToInsideTheRange_BoundTheOutput()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 12, 12));
        Assert.True(rule.End(new DateOnly(2026, 12, 19)).IsSuccess);

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 1), new DateOnly(2026, 12, 31));

        Assert.Equal([new DateOnly(2026, 12, 12), new DateOnly(2026, 12, 19)], sessions.Select(session => session.LocalDate));
        Assert.Equal([Utc(2026, 12, 12, 16, 0), Utc(2026, 12, 19, 16, 0)], sessions.Select(session => session.StartsAt));
    }

    // Case 4
    [Fact]
    public void Generate_AcrossSpringForward_WallClockStaysEighteenAndUtcMovesAnHourEarlier()
```

The excerpt stops at line 90; use the original file for the remaining 114 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
