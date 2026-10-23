using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: the enrollment use cases and the rules they added to earlier ones, through the real dispatcher,
/// real adapters and real PostgreSQL, plus the audit rows each command writes through the policy alone.
/// </summary>
public sealed class EnrollmentUseCaseTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    // Names only: no phone or any other contact field of a student or parent may appear on a roster entry.
    private static readonly string[] RosterEntryFields = ["EnrolledAt", "EnrollmentId", "StudentId", "StudentName", "Version"];

    [Fact]
    public async Task Withdraw_FreesTheSeatThatTheNextEnrollmentTakes()
    {
        var world = await ArrangeAsync("uc1", capacity: 1);
        var first = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        Assert.Equal("group.full", (await EnrollAsync(world.Secretary.Actor, world.GroupId, world.Students[1])).Error!.Code);

        var withdrawn = await WithdrawAsync(world, first);
        var second = await EnrollAsync(world.Secretary.Actor, world.GroupId, world.Students[1]);

        Assert.True(withdrawn.IsSuccess, withdrawn.Error?.Message);
        Assert.True(second.IsSuccess, second.Error?.Message);
        Assert.Equal(1, await ActiveRowsAsync(world.GroupId));
        Assert.Equal(
            "ended|withdrawn",
            await Fixture.ScalarAsync<string>($"select concat_ws('|', status, end_reason) from academics.enrollments where id = '{first}'"));
    }

    [Fact]
    public async Task ReEnrollAfterWithdrawal_CreatesASecondRowAndKeepsTheFirstAsHistory()
    {
        var world = await ArrangeAsync("uc2");
        var first = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        Assert.True((await WithdrawAsync(world, first)).IsSuccess);

        var second = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        Assert.NotEqual(first, second);
        Assert.Equal(
            2,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.enrollments where student_id = '{world.Students[0]}' and group_id = '{world.GroupId}'"));
        Assert.Equal(1, await ActiveRowsAsync(world.GroupId));
        var history = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            world.Secretary.Actor, new ListStudentEnrollmentsQuery(world.Students[0]));
        Assert.Equal([second, first], history.Value.Select(enrollment => enrollment.EnrollmentId).ToArray());
        Assert.Equal(EnrollmentStatus.Active, history.Value[0].Status);
        Assert.Equal(EnrollmentEndReason.Withdrawn, history.Value[1].EndReason);
    }

    [Fact]
    public async Task ArchiveGroup_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()
    {
        var world = await ArrangeAsync("uc3");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        var refused = await ArchiveGroupAsync(world);
        Assert.Equal("group.has_active_enrollments", refused.Error!.Code);
        Assert.Equal("active", await Fixture.ScalarAsync<string>($"select status from academics.groups where id = '{world.GroupId}'"));

        Assert.True((await WithdrawAsync(world, enrollmentId)).IsSuccess);
        var archived = await ArchiveGroupAsync(world);
        Assert.True(archived.IsSuccess, archived.Error?.Message);
    }

    [Fact]
    public async Task ArchiveStudent_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()
    {
        var world = await ArrangeAsync("uc4");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        var refused = await ArchiveStudentAsync(world, world.Students[0]);
        Assert.Equal("student.has_active_enrollments", refused.Error!.Code);
        Assert.Equal("active", await Fixture.ScalarAsync<string>($"select status from academics.students where id = '{world.Students[0]}'"));

        Assert.True((await WithdrawAsync(world, enrollmentId)).IsSuccess);
        var archived = await ArchiveStudentAsync(world, world.Students[0]);
        Assert.True(archived.IsSuccess, archived.Error?.Message);
    }

    [Fact]
    public async Task ChangeCapacity_BelowTheEnrolledCount_IsRefusedAndEqualToItSucceeds()
    {
        var world = await ArrangeAsync("uc5");
        await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[1]);

        var below = await ChangeCapacityAsync(world, 1);
        var equal = await ChangeCapacityAsync(world, 2);

        Assert.Equal("group.capacity_below_enrolled", below.Error!.Code);
        Assert.True(equal.IsSuccess, equal.Error?.Message);
        Assert.Equal(2, await CapacityAsync(world.GroupId));
    }

    [Fact]
    public async Task GroupListDetailAndRoster_ShowTheRealEnrolledCountAndNamesOnly()
    {
        var world = await ArrangeAsync("uc6");
        await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[1]);
        await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        var withdrawn = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[2]);
        Assert.True((await WithdrawAsync(world, withdrawn)).IsSuccess);

        var detail = await GetGroupAsync(world.Secretary.Actor, world.GroupId);
        var list = await Fixture.QueryAsAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(world.Secretary.Actor, new ListGroupsQuery(null, null, false));
        var roster = await GetRosterAsync(world.Secretary.Actor, world.GroupId);

        Assert.Equal(2, detail.EnrolledCount);
        Assert.Equal(2, Assert.Single(list.Value).EnrolledCount);
        Assert.Equal(2, roster.EnrolledCount);
        Assert.Equal(20, roster.Capacity);
        Assert.Equal([world.Students[0], world.Students[1]], roster.Items.Select(item => item.StudentId).ToArray());
        Assert.Equal(RosterEntryFields, typeof(RosterEntryDto).GetProperties().Select(property => property.Name).Order().ToArray());
    }

    [Fact]
    public async Task Enroll_WritesOneCreatedAuditRow()
    {
        var world = await ArrangeAsync("audit1");

        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        var filter = $"from audit.audit_entries where entity_type = 'enrollment' and entity_id = '{enrollmentId}'";
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) {filter}"));
        Assert.Equal("created", await Fixture.ScalarAsync<string>($"select action {filter}"));
        Assert.Equal(world.Students[0].ToString(), await Fixture.ScalarAsync<string>($"select changes->'studentId'->>'after' {filter}"));
        Assert.Equal(world.GroupId.ToString(), await Fixture.ScalarAsync<string>($"select changes->'groupId'->>'after' {filter}"));
        Assert.Equal(world.Secretary.UserId, await Fixture.ScalarAsync<Guid>($"select actor_user_id {filter}"));
    }

    [Fact]
    public async Task Withdraw_WritesOneUpdatedAuditRowFromActiveToEndedWithTheReason()
    {
        var world = await ArrangeAsync("audit2");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        Assert.True((await WithdrawAsync(world, enrollmentId)).IsSuccess);

        var filter = $"from audit.audit_entries where entity_type = 'enrollment' and entity_id = '{enrollmentId}' and action = 'updated'";
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) {filter}"));
        Assert.Equal("active", await Fixture.ScalarAsync<string>($"select changes->'status'->>'before' {filter}"));
        Assert.Equal("ended", await Fixture.ScalarAsync<string>($"select changes->'status'->>'after' {filter}"));
        Assert.Equal("withdrawn", await Fixture.ScalarAsync<string>($"select changes->'endReason'->>'after' {filter}"));
    }

    [Fact]
    public async Task Transfer_WritesAnUpdatedRowForTheOldEnrollmentAndACreatedRowForTheNewOne()
    {
        var world = await ArrangeAsync("audit3");
        var targetId = await CreateGroupAsync(world.Secretary.Actor, "Grade 10 B", world.SubjectId, world.TeacherId);
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        var version = (await GetStudentEnrollmentAsync(world.Secretary.Actor, world.Students[0], enrollmentId)).Version;

        var moved = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            world.Secretary.Actor, new TransferEnrollmentCommand(enrollmentId, targetId, version));

        Assert.True(moved.IsSuccess, moved.Error?.Message);
        Assert.Equal(
            "transferred",
            await Fixture.ScalarAsync<string>(
                $"select changes->'endReason'->>'after' from audit.audit_entries where entity_id = '{enrollmentId}' and action = 'updated'"));
        Assert.Equal(
            targetId.ToString(),
            await Fixture.ScalarAsync<string>(
                $"select changes->'groupId'->>'after' from audit.audit_entries where entity_id = '{moved.Value.EnrollmentId}' and action = 'created'"));
    }

    private async Task<Result<Unit>> WithdrawAsync(World world, Guid enrollmentId)
    {
        var studentId = await Fixture.ScalarAsync<Guid>($"select student_id from academics.enrollments where id = '{enrollmentId}'");
        var version = (await GetStudentEnrollmentAsync(world.Secretary.Actor, studentId, enrollmentId)).Version;
        return await Fixture.SendAsAsync<WithdrawEnrollmentCommand, Unit>(world.Secretary.Actor, new WithdrawEnrollmentCommand(enrollmentId, version));
    }

    private async Task<Result<Unit>> ArchiveGroupAsync(World world) =>
        await Fixture.SendAsAsync<ArchiveGroupCommand, Unit>(
            world.Secretary.Actor, new ArchiveGroupCommand(world.GroupId, (await GetGroupAsync(world.Secretary.Actor, world.GroupId)).Version));

    private async Task<Result<Unit>> ChangeCapacityAsync(World world, int capacity) =>
        await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(
            world.Secretary.Actor,
            new ChangeGroupCapacityCommand(world.GroupId, capacity, (await GetGroupAsync(world.Secretary.Actor, world.GroupId)).Version));

    private async Task<Result<Unit>> ArchiveStudentAsync(World world, Guid studentId)
    {
        var student = await Fixture.QueryAsAsync<GetStudentQuery, StudentDetailDto>(world.Secretary.Actor, new GetStudentQuery(studentId));
        Assert.True(student.IsSuccess, student.Error?.Message);
        return await Fixture.SendAsAsync<ArchiveStudentCommand, Unit>(world.Secretary.Actor, new ArchiveStudentCommand(studentId, student.Value.Version));
    }

    private async Task<World> ArrangeAsync(string tag, int capacity = 20)
    {
        var secretary = await CreateStaffAsync(NileCentreId, $"secretary-{tag}@nile.test", StaffRole.Secretary);
        var subjectId = await CreateSubjectAsync(secretary.Actor, "Mathematics");
        var owner = await CreateStaffAsync(NileCentreId, $"owner-{tag}@nile.test", StaffRole.Owner);
        var teacherId = await CreateTeacherAsync(owner.Actor, "Amina Hassan");
        var groupId = await CreateGroupAsync(secretary.Actor, "Grade 10 A", subjectId, teacherId, capacity);
        var students = await SeedStudentsAsync(NileCentreId, 3, $"Student {tag}");
        return new World(secretary, subjectId, teacherId, groupId, students);
    }

    private sealed record World(CreatedStaff Secretary, Guid SubjectId, Guid TeacherId, Guid GroupId, IReadOnlyList<Guid> Students);
}
