# `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 52.11: teacher ownership through the real provider, read service and PostgreSQL. In Nile, G1 and G2 belong to T1 and G3 to T2; the actor is a Teacher-role member linked to T1, and every group has a rule.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 52 lines and 2,535 bytes. Source hash: `eaec938a45caee060920912620923397fb7a1a45b20f8d21a21556c1e7ca2095`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 3: `using TutoringCentre.Domain.Identity;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs#L12): `public sealed class ScheduleTeacherScopeTests(PostgresFixture fixture) : ScheduleTestBase(fixture)`
- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs#L15): `public async Task LinkedTeacher_TimetableContainsOnlyTheirOwnSessions()`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs#L28): `public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_IsGroupNotFound()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: teacher ownership through the real provider, read service and PostgreSQL. In Nile, G1 and G2 belong
/// to T1 and G3 to T2; the actor is a Teacher-role member linked to T1, and every group has a rule.
/// </summary>
public sealed class ScheduleTeacherScopeTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    [Fact]
    public async Task LinkedTeacher_TimetableContainsOnlyTheirOwnSessions()
    {
        var (world, teacherMember) = await ArrangeAsync();

        var asTeacher = await GetTimetableAsync(teacherMember.Actor, new DateOnly(2027, 3, 6));
        var asOwner = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 3, 6));

        Assert.Equal(2, asTeacher.Sessions.Count);
        Assert.All(asTeacher.Sessions, session => Assert.Equal(world.T1, session.TeacherId));
        Assert.Equal(3, asOwner.Sessions.Count);
    }

    [Fact]
    public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_IsGroupNotFound()
    {
        var (world, teacherMember) = await ArrangeAsync();

        var own = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            teacherMember.Actor, new ListGroupScheduleRulesQuery(world.G1));
        var other = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            teacherMember.Actor, new ListGroupScheduleRulesQuery(world.G3));

        Assert.True(own.IsSuccess, own.Error?.Message);
        Assert.Single(own.Value);
        Assert.True(other.IsFailure);
        Assert.Equal("group.not_found", other.Error!.Code);
    }

    private async Task<(World World, CreatedStaff TeacherMember)> ArrangeAsync()
    {
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher1@nile.test", StaffRole.Teacher);
        var world = await ArrangeWorldAsync(NileCentreId, "nile", teacherMember.UserId);
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, new TimeOnly(10, 0));
        await AddRuleOkAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, new TimeOnly(12, 0));
        await AddRuleOkAsync(world.Owner.Actor, world.G3, DayOfWeek.Saturday, new TimeOnly(10, 0));
        return (world, teacherMember);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
