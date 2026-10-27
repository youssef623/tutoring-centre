# `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs`

## Purpose

Source summary (intent, not independent execution proof): Shared setup for Task 52.11's schedule tests. Every schedule use case runs through `Clocked`: the production container with one change, a `FakeTimeProvider` fixed by default at Monday 1 March 2027, 08:00 UTC (10:00 in Cairo), so "today" in the centre is 1 March and no test depends on the real date. References (staff, subjects, teachers, groups) are created through the real use cases; rows that test the database itself are inserted as the superuser. Every assertion on persisted state reads through the superuser connection.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 170 lines and 9,002 bytes. Source hash: `36d234ba2d3d26b9fa6a5d5d1c009af0302d532178fb09e543a115febdc22233`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using Microsoft.Extensions.Time.Testing;`
- Line 3: `using Npgsql;`
- Line 4: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 5: `using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;`
- Line 6: `using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;`
- Line 7: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 8: `using TutoringCentre.Application.Common.Security;`
- Line 9: `using TutoringCentre.Domain.Common;`
- Line 10: `using TutoringCentre.Domain.Identity;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L22): `public abstract class ScheduleTestBase(PostgresFixture fixture) : EnrollmentTestBase(fixture), IAsyncLifetime`
- [Line 47](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L47): `private protected async Task<World> ArrangeWorldAsync(Guid centreId, string tag, Guid? t1UserId = null)`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L59): `private protected Task<Result<AddScheduleRuleResult>> AddRuleAsync(`
- [Line 64](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L64): `private protected async Task<AddScheduleRuleResult> AddRuleOkAsync(`
- [Line 72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L72): `private protected async Task<ScheduleRuleDto> GetRuleAsync(Actor actor, Guid groupId, Guid ruleId)`
- [Line 80](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L80): `private protected async Task<TimetableDto> GetTimetableAsync(Actor actor, DateOnly weekStart, Guid? groupId = null, Guid? teacherId = null)`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L87): `private protected Task<long> CountAsync(string sql) => Fixture.ScalarAsync<long>(sql);`
- [Line 89](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L89): `private protected Task<long> SessionCountAsync(string where = "true") =>`
- [Line 92](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L92): `private protected Task<long> AuditCountAsync(string entityType, string action) =>`
- [Line 96](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L96): `private protected async Task<Guid> SeedRuleAsync(Guid centreId, Guid groupId, DateOnly? effectiveFrom = null)`
- [Line 121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L121): `private protected async Task<Guid> InsertSessionAsync(`
- [Line 157](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L157): `private protected static DateTimeOffset At(int hour, int minute = 0, int day = 6) => new(2027, 3, day, hour, minute, 0, TimeSpan.Zero);`
- [Line 159](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L159): `private protected sealed record World(CreatedStaff Owner, Guid T1, Guid T2, Guid G1, Guid G2, Guid G3);`
- [Line 161](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L161): `private protected sealed record SessionRow(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs:9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs#L9)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs:11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L11)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs:131](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L131)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs:15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L20)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:158](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L158)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs:11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L11)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Npgsql;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Shared setup for Task 52.11's schedule tests. Every schedule use case runs through <see cref="Clocked"/>: the
/// production container with one change, a <see cref="FakeTimeProvider"/> fixed by default at Monday 1 March 2027,
/// 08:00 UTC (10:00 in Cairo), so "today" in the centre is 1 March and no test depends on the real date. References
/// (staff, subjects, teachers, groups) are created through the real use cases; rows that test the database itself
/// are inserted as the superuser. Every assertion on persisted state reads through the superuser connection.
/// </summary>
public abstract class ScheduleTestBase(PostgresFixture fixture) : EnrollmentTestBase(fixture), IAsyncLifetime
{
    private protected static readonly DateTimeOffset DefaultNow = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);
    private protected static readonly DateOnly DefaultToday = new(2027, 3, 1);
    private protected static readonly TimeOnly SixPm = new(18, 0);

    private ServiceProvider? _clocked;

    private protected FakeTimeProvider Time { get; } = new(DefaultNow);

    /// <summary>The production container with <see cref="Time"/> as its clock; built on first use, disposed after the test.</summary>
    private protected ServiceProvider Clocked =>
        _clocked ??= Fixture.CreateServiceProvider(services => services.AddSingleton<TimeProvider>(Time));

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_clocked is not null)
        {
            await _clocked.DisposeAsync();
        }

        await DisposeAsync();
    }

    /// <summary>An owner, one subject, teachers T1 and T2, groups G1 and G2 taught by T1 and G3 taught by T2.</summary>
    private protected async Task<World> ArrangeWorldAsync(Guid centreId, string tag, Guid? t1UserId = null)
    {
        var owner = await CreateStaffAsync(centreId, $"owner-{tag}@{tag}.test", StaffRole.Owner);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var t1 = await CreateTeacherAsync(owner.Actor, "Amina Hassan", t1UserId);
        var t2 = await CreateTeacherAsync(owner.Actor, "Youssef Kamal");
        var g1 = await CreateGroupAsync(owner.Actor, "Grade 10 A", subjectId, t1);
        var g2 = await CreateGroupAsync(owner.Actor, "Grade 10 B", subjectId, t1);
        var g3 = await CreateGroupAsync(owner.Actor, "Grade 11 A", subjectId, t2);
        return new World(owner, t1, t2, g1, g2, g3);
    }

    private protected Task<Result<AddScheduleRuleResult>> AddRuleAsync(
        Actor actor, Guid groupId, DayOfWeek day, TimeOnly start, int durationMinutes = 90, DateOnly? effectiveFrom = null) =>
        Clocked.SendAsAsync<AddScheduleRuleCommand, AddScheduleRuleResult>(
            actor, new AddScheduleRuleCommand(groupId, day, start, durationMinutes, "Room 1", effectiveFrom ?? DefaultToday));

    private protected async Task<AddScheduleRuleResult> AddRuleOkAsync(
        Actor actor, Guid groupId, DayOfWeek day, TimeOnly start, int durationMinutes = 90, DateOnly? effectiveFrom = null)
    {
        var result = await AddRuleAsync(actor, groupId, day, start, durationMinutes, effectiveFrom);
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private protected async Task<ScheduleRuleDto> GetRuleAsync(Actor actor, Guid groupId, Guid ruleId)
    {
        var result = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            actor, new ListGroupScheduleRulesQuery(groupId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.Single(rule => rule.Id == ruleId);
    }

    private protected async Task<TimetableDto> GetTimetableAsync(Actor actor, DateOnly weekStart, Guid? groupId = null, Guid? teacherId = null)
    {
        var result = await Clocked.QueryAsAsync<GetTimetableQuery, TimetableDto>(actor, new GetTimetableQuery(weekStart, groupId, teacherId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private protected Task<long> CountAsync(string sql) => Fixture.ScalarAsync<long>(sql);

    private protected Task<long> SessionCountAsync(string where = "true") =>
        CountAsync($"select count(*) from academics.class_sessions where {where}");
```

The excerpt stops at line 90; use the original file for the remaining 80 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
