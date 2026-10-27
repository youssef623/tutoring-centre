# `tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: tenancy and permission. Owner and Secretary hold schedule.manage; Teacher holds only schedule.view, so the four commands are denied for Teacher before any handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is refused every schedule request as tenant.not_selected.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 186 lines and 9,082 bytes. Source hash: `b045baa940625ea6733f6a6aa73c041ae87c21d7b6e709f3b516aa75b180a7b6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Groups;`
- Line 5: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 6: `using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;`
- Line 7: `using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;`
- Line 8: `using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;`
- Line 9: `using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;`
- Line 10: `using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;`
- Line 11: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 12: `using TutoringCentre.Application.Centres.Contracts;`
- Line 13: `using TutoringCentre.Application.Common.Cqrs;`
- Line 14: `using TutoringCentre.Application.Common.Ports;`
- Line 15: `using TutoringCentre.Application.Common.Security;`
- Line 16: `using TutoringCentre.Application.Tests.Fakes;`
- Line 17: `using TutoringCentre.Domain.Academics;`
- Line 18: `using TutoringCentre.Domain.Common;`
- Line 19: `using TutoringCentre.Domain.Identity;`
- Line 20: `using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L30): `public sealed class ScheduleDispatcherPermissionTests`
- [Line 42](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L42): `public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)`
- [Line 58](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L58): `public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)`
- [Line 72](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L72): `public async Task SendAsync_AddRule_SucceedsForOwnerAndSecretary(StaffRole role)`
- [Line 83](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L83): `public async Task QueryAsync_TimetableAndRules_AreAllowedForTeacher()`
- [Line 98](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L98): `public async Task QueryAsync_TimetableAndRules_ReturnTenantNotSelectedForACentrelessActor()`
- [Line 112](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L112): `private static async Task<Error?> SendAsync(Fixture fixture, string commandName)`
- [Line 131](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L131): `private static Fixture CreateSut(Actor actor)`
- [Line 166](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L166): `private sealed class Fixture(`
- [Line 184](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L184): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: tenancy and permission. Owner and Secretary hold schedule.manage; Teacher holds
/// only schedule.view, so the four commands are denied for Teacher before any handler or transaction runs, while
/// both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is
/// refused every schedule request as tenant.not_selected.
/// </summary>
public sealed class ScheduleDispatcherPermissionTests
{
    public static TheoryData<string> CommandNames => new()
    {
        nameof(AddScheduleRuleCommand),
        nameof(EndScheduleRuleCommand),
        nameof(ExtendScheduleCommand),
        nameof(CancelSessionCommand),
    };

    [Theory]
    [MemberData(nameof(CommandNames))]
    public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var error = await SendAsync(fixture, commandName);

        Assert.NotNull(error);
        Assert.Equal("auth.permission_denied", error.Code);
        Assert.Equal(ErrorKind.Forbidden, error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
        Assert.Empty(fixture.Rules.Added);
        Assert.Empty(fixture.Sessions.Added);
    }

    [Theory]
    [MemberData(nameof(CommandNames))]
    public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), null, null));

        var error = await SendAsync(fixture, commandName);

        Assert.NotNull(error);
        Assert.Equal("tenant.not_selected", error.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    public async Task SendAsync_AddRule_SucceedsForOwnerAndSecretary(StaffRole role)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, role));

        var error = await SendAsync(fixture, nameof(AddScheduleRuleCommand));

        Assert.Null(error);
        Assert.Single(fixture.Rules.Added);
    }

    [Fact]
    public async Task QueryAsync_TimetableAndRules_AreAllowedForTeacher()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var timetable = await fixture.Dispatcher.QueryAsync<GetTimetableQuery, TimetableDto>(
            new GetTimetableQuery(Today, null, null), CancellationToken.None);
        var rules = await fixture.Dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            new ListGroupScheduleRulesQuery(Guid.CreateVersion7()), CancellationToken.None);
```

The excerpt stops at line 90; use the original file for the remaining 96 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
