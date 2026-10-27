# `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 52.11: daylight saving time as stored and as selected. Cairo moves from UTC+2 to UTC+3 at 00:00 on Friday 30 April 2027 (the clocks jump to 01:00). "Today" is Tuesday 20 April 2027, before the change.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 59 lines and 3,054 bytes. Source hash: `0ba33927b8cd45cee5388df07789d52d0ba146c8b1023b39121bb42ae7274bec`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs#L9): `public sealed class ScheduleDstPersistenceTests(PostgresFixture fixture) : ScheduleTestBase(fixture)`
- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs#L14): `public async Task SaturdayAtSixPm_IsStoredAtSixteenUtcBeforeTheChangeAndFifteenUtcAfterIt()`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs#L30): `public async Task FridayAtHalfPastMidnight_OnTheChangeDay_IsInTheTimetableUnderItsLocalDateNotItsUtcDate()`
- [Line 53](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs#L53): `private async Task<DateTimeOffset> StartsAtAsync(Guid ruleId, DateOnly localDate)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: daylight saving time as stored and as selected. Cairo moves from UTC+2 to UTC+3 at 00:00 on Friday
/// 30 April 2027 (the clocks jump to 01:00). "Today" is Tuesday 20 April 2027, before the change.
/// </summary>
public sealed class ScheduleDstPersistenceTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private static readonly DateTimeOffset TwentiethOfApril = new(2027, 4, 20, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task SaturdayAtSixPm_IsStoredAtSixteenUtcBeforeTheChangeAndFifteenUtcAfterIt()
    {
        Time.SetUtcNow(TwentiethOfApril);
        var world = await ArrangeWorldAsync(NileCentreId, "nile");

        var added = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm, effectiveFrom: new DateOnly(2027, 4, 20));

        Assert.Equal(
            new DateTimeOffset(2027, 4, 24, 16, 0, 0, TimeSpan.Zero),
            await StartsAtAsync(added.ScheduleRuleId, new DateOnly(2027, 4, 24)));
        Assert.Equal(
            new DateTimeOffset(2027, 5, 1, 15, 0, 0, TimeSpan.Zero),
            await StartsAtAsync(added.ScheduleRuleId, new DateOnly(2027, 5, 1)));
    }

    [Fact]
    public async Task FridayAtHalfPastMidnight_OnTheChangeDay_IsInTheTimetableUnderItsLocalDateNotItsUtcDate()
    {
        Time.SetUtcNow(TwentiethOfApril);
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        var added = await AddRuleOkAsync(
            world.Owner.Actor, world.G1, DayOfWeek.Friday, new TimeOnly(0, 30), effectiveFrom: new DateOnly(2027, 4, 20));
        var thirtiethOfApril = new DateOnly(2027, 4, 30);

        // 00:30 does not exist on 30 April (00:00 jumps to 01:00), so the session starts at 01:30 UTC+3: 22:30 UTC
        // on 29 April. Its UTC date is the 29th; its local date is the 30th.
        var startsAt = await StartsAtAsync(added.ScheduleRuleId, thirtiethOfApril);
        Assert.Equal(new DateTimeOffset(2027, 4, 29, 22, 30, 0, TimeSpan.Zero), startsAt);

        var weekWithThe30th = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 4, 24));
        var weekEndingThe29th = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 4, 23));

        var session = Assert.Single(weekWithThe30th.Sessions);
        Assert.Equal(thirtiethOfApril, session.LocalDate);
        Assert.Equal(startsAt, session.StartsAt);
        Assert.Equal("Africa/Cairo", weekWithThe30th.TimeZoneId);
        Assert.DoesNotContain(weekEndingThe29th.Sessions, other => other.LocalDate == thirtiethOfApril);
    }

    private async Task<DateTimeOffset> StartsAtAsync(Guid ruleId, DateOnly localDate)
    {
        var startsAt = await Fixture.ScalarAsync<DateTime>(
            $"select starts_at from academics.class_sessions where schedule_rule_id = '{ruleId}' and local_date = '{localDate:yyyy-MM-dd}'");
        return new DateTimeOffset(DateTime.SpecifyKind(startsAt, DateTimeKind.Utc));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
