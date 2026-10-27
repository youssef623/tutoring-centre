# `tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs`

## Purpose

Declares `AddScheduleRuleHandlerTests`, `Sut`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 158 lines and 6,341 bytes. Source hash: `ed7360a8e4970360014c35ee039d3b07485fcfc9eed3ab47805fa3ec6a6ef85d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L11): `public sealed class AddScheduleRuleHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L14): `public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFoundAndAddsNothing()`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L24): `public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchivedAndAddsNothing()`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L35): `public async Task HandleAsync_EffectiveFromMoreThan366DaysAhead_ReturnsRangeInvalidOnEffectiveFrom()`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L46): `public async Task HandleAsync_EffectiveFromExactly366DaysAhead_AddsTheRuleWithNoSessionsYet()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L59): `public async Task HandleAsync_RuleFromToday_AddsTheRuleAndFourWeeksOfSessionsForTheGroupsTeacher()`
- [Line 86](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L86): `public async Task HandleAsync_TakesTheTeachersScheduleLockBeforeAddingAnySession()`
- [Line 97](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L97): `public async Task HandleAsync_RuleThatTookEffectInThePast_GeneratesNoDateBeforeToday()`
- [Line 109](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L109): `public async Task HandleAsync_RuleFromLaterInTheHorizon_GeneratesFromItsFirstDateOnly()`
- [Line 122](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L122): `public async Task HandleAsync_DurationOffTheFiveMinuteStep_ReturnsTheRulesOwnError()`
- [Line 132](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L132): `private static AddScheduleRuleCommand Command(Guid groupId, DateOnly? effectiveFrom = null) =>`
- [Line 135](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L135): `private static void AssertFailure(Result result, string code, ErrorKind kind, Sut sut)`
- [Line 144](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L144): `private static Sut CreateSut()`
- [Line 157](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L157): `private sealed record Sut(AddScheduleRuleHandler Handler, Group Group, FakeScheduleRuleRepository Rules, FakeClassSessionRepository Sessions);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class AddScheduleRuleHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFoundAndAddsNothing()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(Guid.CreateVersion7()), CancellationToken.None);

        AssertFailure(result, "group.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchivedAndAddsNothing()
    {
        var sut = CreateSut();
        sut.Group.Archive();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id), CancellationToken.None);

        AssertFailure(result, "group.archived", ErrorKind.Rule, sut);
    }

    [Fact]
    public async Task HandleAsync_EffectiveFromMoreThan366DaysAhead_ReturnsRangeInvalidOnEffectiveFrom()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id, Today.AddDays(367)), CancellationToken.None);

        AssertFailure(result, "schedule.range_invalid", ErrorKind.Rule, sut);
        Assert.Contains("effectiveFrom", result.Error!.Fields!.Keys);
    }

    [Fact]
    public async Task HandleAsync_EffectiveFromExactly366DaysAhead_AddsTheRuleWithNoSessionsYet()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id, Today.AddDays(366)), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Single(sut.Rules.Added);
        Assert.Equal(0, result.Value.SessionsCreated);
        Assert.Empty(sut.Sessions.Added);
    }

    [Fact]
    public async Task HandleAsync_RuleFromToday_AddsTheRuleAndFourWeeksOfSessionsForTheGroupsTeacher()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        var rule = Assert.Single(sut.Rules.Added);
        Assert.Equal(rule.Id, result.Value.ScheduleRuleId);
        Assert.Equal(CentreId, rule.CentreId);
        Assert.Equal(4, result.Value.SessionsCreated);
        Assert.Equal(
            [new DateOnly(2027, 3, 6), new DateOnly(2027, 3, 13), new DateOnly(2027, 3, 20), new DateOnly(2027, 3, 27)],
            sut.Sessions.Added.Select(session => session.LocalDate).ToArray());
        Assert.All(sut.Sessions.Added, session =>
        {
            Assert.Equal(sut.Group.TeacherId, session.TeacherId);
            Assert.Equal(rule.Id, session.ScheduleRuleId);
        });

        // 18:00 in Cairo's winter (UTC+2) is 16:00 UTC; 90 minutes later is 17:30 UTC.
        var first = sut.Sessions.Added[0];
        Assert.Equal(new DateTimeOffset(2027, 3, 6, 16, 0, 0, TimeSpan.Zero), first.StartsAt);
        Assert.Equal(new DateTimeOffset(2027, 3, 6, 17, 30, 0, TimeSpan.Zero), first.EndsAt);
    }

    [Fact]
    public async Task HandleAsync_TakesTheTeachersScheduleLockBeforeAddingAnySession()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id), CancellationToken.None);
```

The excerpt stops at line 90; use the original file for the remaining 68 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
