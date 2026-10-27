# `tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs`

## Purpose

Source summary (intent, not independent execution proof): One Saturday rule whose first four weeks (6-27 March) already exist, as after adding it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 119 lines and 4,478 bytes. Source hash: `2c1af86880e82198c9ec779d16d21be154f66235b724fac42d765ac40a47a195`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L9): `public sealed class ExtendScheduleHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L14): `public async Task HandleAsync_ThroughBeforeToday_ReturnsRangeInvalidOnThrough()`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L24): `public async Task HandleAsync_ThroughMoreThan92DaysAhead_ReturnsRangeInvalidOnThrough()`
- [Line 34](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L34): `public async Task HandleAsync_ThroughExactly92DaysAhead_IsAccepted()`
- [Line 44](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L44): `public async Task HandleAsync_AddsOnlyTheDatesThatHaveNoSessionYet()`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L56): `public async Task HandleAsync_RunTwice_TheSecondRunCreatesNothing()`
- [Line 71](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L71): `public async Task HandleAsync_NewSessionsGoToTheGroupsCurrentTeacher()`
- [Line 83](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L83): `public async Task HandleAsync_TakesTheTeachersScheduleLocksBeforeAddingAnySession()`
- [Line 92](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L92): `private static void AssertRangeInvalid(Result result, Sut sut)`
- [Line 102](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L102): `private static Sut CreateSut()`
- [Line 118](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L118): `private sealed record Sut(ExtendScheduleHandler Handler, Group Group, FakeClassSessionRepository Sessions);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ExtendScheduleHandlerTests
{
    private static readonly DateOnly Saturday10April = new(2027, 4, 10);

    [Fact]
    public async Task HandleAsync_ThroughBeforeToday_ReturnsRangeInvalidOnThrough()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Today.AddDays(-1)), CancellationToken.None);

        AssertRangeInvalid(result, sut);
    }

    [Fact]
    public async Task HandleAsync_ThroughMoreThan92DaysAhead_ReturnsRangeInvalidOnThrough()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Today.AddDays(93)), CancellationToken.None);

        AssertRangeInvalid(result, sut);
    }

    [Fact]
    public async Task HandleAsync_ThroughExactly92DaysAhead_IsAccepted()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Today.AddDays(92)), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [Fact]
    public async Task HandleAsync_AddsOnlyTheDatesThatHaveNoSessionYet()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(2, result.Value.SessionsCreated);
        Assert.Equal([new DateOnly(2027, 4, 3), Saturday10April], sut.Sessions.Added.Select(s => s.LocalDate).ToArray());
    }

    [Fact]
    public async Task HandleAsync_RunTwice_TheSecondRunCreatesNothing()
    {
        var sut = CreateSut();
        await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);
        sut.Sessions.Existing.AddRange(sut.Sessions.Added);
        sut.Sessions.Added.Clear();

        var second = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.True(second.IsSuccess, second.Error?.Message);
        Assert.Equal(0, second.Value.SessionsCreated);
        Assert.Empty(sut.Sessions.Added);
    }

    [Fact]
    public async Task HandleAsync_NewSessionsGoToTheGroupsCurrentTeacher()
    {
        var sut = CreateSut();
        var newTeacherId = Guid.CreateVersion7();
        sut.Group.AssignTeacher(newTeacherId);

        await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.All(sut.Sessions.Added, session => Assert.Equal(newTeacherId, session.TeacherId));
    }

    [Fact]
    public async Task HandleAsync_TakesTheTeachersScheduleLocksBeforeAddingAnySession()
    {
        var sut = CreateSut();

        await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.Equal([(sut.Group.TeacherId, 0)], sut.Sessions.Locked);
    }
```

The excerpt stops at line 90; use the original file for the remaining 29 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
