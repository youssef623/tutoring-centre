# `tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs`

## Purpose

Declares `ScheduleRuleTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 177 lines and 6,784 bytes. Source hash: `73e2f57c659385c96e3c82ccdf5aeffa4b6eaea4e80b9a3ef08a1169fa4674b2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L6): `public sealed class ScheduleRuleTests`
- [Line 16](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L16): `public void Create_WithValidInput_StoresLocalValuesAndIsOpenEnded()`
- [Line 34](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L34): `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- [Line 39](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L39): `public void Create_WithEmptyGroup_ReturnsGroupRequired() =>`
- [Line 49](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L49): `public void Create_DurationBoundaries(int durationMinutes, bool valid)`
- [Line 67](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L67): `public void Create_WithStartTimeOffTheFiveMinuteGrid_ReturnsStartTimeInvalid(int hour, int minute, int second) =>`
- [Line 74](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L74): `public void Create_With41CharacterRoom_ReturnsRoomInvalid() =>`
- [Line 81](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L81): `public void Create_With40CharacterRoom_Succeeds()`
- [Line 96](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L96): `public void Create_WithBlankRoom_StoresNull(string? room)`
- [Line 112](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L112): `public void OccursOn_MatchesTheWeekdayInsideTheEffectiveRange(int year, int month, int day, bool expected)`
- [Line 121](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L121): `public void OccursOn_OpenEndedRule_KeepsOccurringOnTheWeekday()`
- [Line 129](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L129): `public void End_OnTheEffectiveFromDate_Succeeds()`
- [Line 138](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L138): `public void End_BeforeEffectiveFrom_ReturnsRuleEndInvalidAndStaysOpen()`
- [Line 151](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L151): `public void End_Twice_ReturnsRuleAlreadyEndedAndKeepsTheFirstEnd()`
- [Line 165](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L165): `private static void AssertValidationFailure(Result<ScheduleRule> result, string expectedCode, string? field)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics.Scheduling;

public sealed class ScheduleRuleTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly TimeOnly SixPm = new(18, 0);

    // Saturday 5 December 2026.
    private static readonly DateOnly EffectiveFrom = new(2026, 12, 5);

    [Fact]
    public void Create_WithValidInput_StoresLocalValuesAndIsOpenEnded()
    {
        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, "  Room 2  ", EffectiveFrom);

        Assert.True(result.IsSuccess);
        var rule = result.Value;
        Assert.Equal(CentreId, rule.CentreId);
        Assert.Equal(GroupId, rule.GroupId);
        Assert.Equal(DayOfWeek.Saturday, rule.DayOfWeek);
        Assert.Equal(SixPm, rule.StartTime);
        Assert.Equal(90, rule.DurationMinutes);
        Assert.Equal("Room 2", rule.Room);
        Assert.Equal(EffectiveFrom, rule.EffectiveFrom);
        Assert.Null(rule.EffectiveTo);
        Assert.NotEqual(Guid.Empty, rule.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(
            ScheduleRule.Create(Guid.Empty, GroupId, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom), "schedule.centre_required", field: null);

    [Fact]
    public void Create_WithEmptyGroup_ReturnsGroupRequired() =>
        AssertValidationFailure(
            ScheduleRule.Create(CentreId, Guid.Empty, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom), "schedule.group_required", field: null);

    [Theory]
    [InlineData(25, false)]
    [InlineData(30, true)]
    [InlineData(92, false)]
    [InlineData(240, true)]
    [InlineData(245, false)]
    public void Create_DurationBoundaries(int durationMinutes, bool valid)
    {
        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, durationMinutes, null, EffectiveFrom);

        if (valid)
        {
            Assert.True(result.IsSuccess);
            Assert.Equal(durationMinutes, result.Value.DurationMinutes);
        }
        else
        {
            AssertValidationFailure(result, "schedule.duration_invalid", "durationMinutes");
        }
    }

    [Theory]
    [InlineData(18, 2, 0)]
    [InlineData(18, 0, 30)]
    public void Create_WithStartTimeOffTheFiveMinuteGrid_ReturnsStartTimeInvalid(int hour, int minute, int second) =>
        AssertValidationFailure(
            ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, new TimeOnly(hour, minute, second), 90, null, EffectiveFrom),
            "schedule.start_time_invalid",
            "startTime");

    [Fact]
    public void Create_With41CharacterRoom_ReturnsRoomInvalid() =>
        AssertValidationFailure(
            ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, new string('r', 41), EffectiveFrom),
            "schedule.room_invalid",
            "room");

    [Fact]
    public void Create_With40CharacterRoom_Succeeds()
    {
        var room = new string('r', 40);

        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, room, EffectiveFrom);

        Assert.True(result.IsSuccess);
        Assert.Equal(room, result.Value.Room);
        Assert.Equal(40, ScheduleRule.RoomMaxLength);
    }
```

The excerpt stops at line 90; use the original file for the remaining 87 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
