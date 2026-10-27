# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 48.10: ownership for derived resources. A roster and a student's enrollments belong to groups, so the group's teacher scope applies to them too, through the real provider, read service and PostgreSQL. In Nile, G1 belongs to teacher T1 and G2 to teacher T2; the actor is a Teacher-role member linked to T1.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 61 lines and 3,399 bytes. Source hash: `3b13ef17c686113cac31d7fd60b1f68c97a14558fe9e1f57097301f31ef11fd8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L14): `public sealed class EnrollmentTeacherScopeTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L17): `public async Task LinkedTeacher_ReadsTheOwnGroupsRosterAndGetsGroupNotFoundForAnothers()`
- [Line 31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L31): `public async Task LinkedTeacher_ListingAStudentsEnrollments_SeesOnlyThoseInOwnGroups()`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L45): `private async Task<World> ArrangeAsync(string tag)`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L60): `private sealed record World(CreatedStaff Owner, CreatedStaff T1Actor, Guid G1, Guid G2, Guid StudentId);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: ownership for derived resources. A roster and a student's enrollments belong to groups, so the
/// group's teacher scope applies to them too, through the real provider, read service and PostgreSQL. In Nile,
/// G1 belongs to teacher T1 and G2 to teacher T2; the actor is a Teacher-role member linked to T1.
/// </summary>
public sealed class EnrollmentTeacherScopeTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    [Fact]
    public async Task LinkedTeacher_ReadsTheOwnGroupsRosterAndGetsGroupNotFoundForAnothers()
    {
        var world = await ArrangeAsync("rscope1");

        var own = await Fixture.QueryAsAsync<GetGroupRosterQuery, GroupRosterDto>(world.T1Actor.Actor, new GetGroupRosterQuery(world.G1));
        var other = await Fixture.QueryAsAsync<GetGroupRosterQuery, GroupRosterDto>(world.T1Actor.Actor, new GetGroupRosterQuery(world.G2));

        Assert.True(own.IsSuccess, own.Error?.Message);
        Assert.Equal(world.StudentId, Assert.Single(own.Value.Items).StudentId);
        Assert.True(other.IsFailure);
        Assert.Equal("group.not_found", other.Error!.Code);
    }

    [Fact]
    public async Task LinkedTeacher_ListingAStudentsEnrollments_SeesOnlyThoseInOwnGroups()
    {
        var world = await ArrangeAsync("rscope2");

        var asTeacher = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            world.T1Actor.Actor, new ListStudentEnrollmentsQuery(world.StudentId));
        var asOwner = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            world.Owner.Actor, new ListStudentEnrollmentsQuery(world.StudentId));

        Assert.True(asTeacher.IsSuccess, asTeacher.Error?.Message);
        Assert.Equal(world.G1, Assert.Single(asTeacher.Value).GroupId);
        Assert.Equal([world.G1, world.G2], asOwner.Value.Select(enrollment => enrollment.GroupId).Order().ToArray());
    }

    private async Task<World> ArrangeAsync(string tag)
    {
        var owner = await CreateStaffAsync(NileCentreId, $"owner-{tag}@nile.test", StaffRole.Owner);
        var t1Member = await CreateStaffAsync(NileCentreId, $"teacher1-{tag}@nile.test", StaffRole.Teacher);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var t1 = await CreateTeacherAsync(owner.Actor, "Amina Hassan", t1Member.UserId);
        var t2 = await CreateTeacherAsync(owner.Actor, "Youssef Kamal");
        var g1 = await CreateGroupAsync(owner.Actor, "Grade 10 A", subjectId, t1);
        var g2 = await CreateGroupAsync(owner.Actor, "Grade 10 B", subjectId, t2);
        var studentId = (await SeedStudentsAsync(NileCentreId, 1, $"Student {tag}"))[0];
        await EnrollOkAsync(owner.Actor, g1, studentId);
        await EnrollOkAsync(owner.Actor, g2, studentId);
        return new World(owner, t1Member, g1, g2, studentId);
    }

    private sealed record World(CreatedStaff Owner, CreatedStaff T1Actor, Guid G1, Guid G2, Guid StudentId);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
