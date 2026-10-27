# `tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs`

## Purpose

Declares `EndScheduleRuleHandlerTests`, `Sut`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 94 lines and 4,219 bytes. Source hash: `2bf4e882a6f5bb361b123f864522522b52409ac2f21566d356ae77d9bf924be0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L9): `public sealed class EndScheduleRuleHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L14): `public async Task HandleAsync_UnknownRule_ReturnsRuleNotFound()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L26): `public async Task HandleAsync_EndsTheRuleAndCancelsOnlyTheSessionsAfterTheLastDate()`
- [Line 41](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L41): `public async Task HandleAsync_ASessionAfterTheLastDateThatHasStarted_IsLeftScheduled()`
- [Line 54](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L54): `public async Task HandleAsync_AlreadyEndedRule_ReturnsRuleAlreadyEndedAndCancelsNothing()`
- [Line 67](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L67): `public async Task HandleAsync_LastDateBeforeTheRuleTakesEffect_ReturnsRuleEndInvalid()`
- [Line 79](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L79): `private static Sut CreateSut(DateTimeOffset? now = null)`
- [Line 93](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L93): `private sealed record Sut(EndScheduleRuleHandler Handler, ScheduleRule Rule, FakeClassSessionRepository Sessions);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class EndScheduleRuleHandlerTests
{
    private static readonly DateOnly Saturday13March = new(2027, 3, 13);

    [Fact]
    public async Task HandleAsync_UnknownRule_ReturnsRuleNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(Guid.CreateVersion7(), Saturday13March, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_EndsTheRuleAndCancelsOnlyTheSessionsAfterTheLastDate()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, Saturday13March, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(Saturday13March, sut.Rule.EffectiveTo);
        Assert.All(sut.Sessions.Existing.Where(s => s.LocalDate <= Saturday13March), s => Assert.Equal(ClassSessionStatus.Scheduled, s.Status));
        Assert.All(sut.Sessions.Existing.Where(s => s.LocalDate > Saturday13March), s => Assert.Equal(ClassSessionStatus.Cancelled, s.Status));
        Assert.Equal(2, sut.Sessions.Existing.Count(s => s.Status == ClassSessionStatus.Cancelled));
        Assert.Empty(sut.Sessions.Added);
    }

    [Fact]
    public async Task HandleAsync_ASessionAfterTheLastDateThatHasStarted_IsLeftScheduled()
    {
        // Now is 6 March 16:30 UTC: the 6 March session (16:00-17:30 UTC) is under way.
        var sut = CreateSut(new DateTimeOffset(2027, 3, 6, 16, 30, 0, TimeSpan.Zero));

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, new DateOnly(2027, 3, 5), 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(ClassSessionStatus.Scheduled, sut.Sessions.Existing.Single(s => s.LocalDate == new DateOnly(2027, 3, 6)).Status);
        Assert.Equal(3, sut.Sessions.Existing.Count(s => s.Status == ClassSessionStatus.Cancelled));
    }

    [Fact]
    public async Task HandleAsync_AlreadyEndedRule_ReturnsRuleAlreadyEndedAndCancelsNothing()
    {
        var sut = CreateSut();
        sut.Rule.End(new DateOnly(2027, 3, 27));

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, Saturday13March, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_already_ended", result.Error!.Code);
        Assert.All(sut.Sessions.Existing, s => Assert.Equal(ClassSessionStatus.Scheduled, s.Status));
    }

    [Fact]
    public async Task HandleAsync_LastDateBeforeTheRuleTakesEffect_ReturnsRuleEndInvalid()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, Today.AddDays(-1), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_end_invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Null(sut.Rule.EffectiveTo);
    }

    private static Sut CreateSut(DateTimeOffset? now = null)
    {
        var teacherId = Guid.CreateVersion7();
        var rule = SaturdayRule(Guid.CreateVersion7());
        var rules = new FakeScheduleRuleRepository();
        rules.Existing.Add(rule);
        var sessions = new FakeClassSessionRepository();
        sessions.Existing.AddRange(Sessions(rule, teacherId, Today, Today.AddDays(27)));

        var clock = Clock();
        clock.UtcNow = now ?? Now;
        return new Sut(new EndScheduleRuleHandler(clock, rules, sessions), rule, sessions);
```

The excerpt stops at line 90; use the original file for the remaining 4 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
