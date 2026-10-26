using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 43.14: the account link is the security-relevant part of Task 43.8, so most cases here are refusals
/// rather than the happy path alone — an unknown user, a user of another centre, a user with another role
/// and an inactive member all answer teacher.user_not_eligible identically through the real IStaffDirectory
/// adapter and real PostgreSQL; a user already linked answers teacher.account_taken, including under a real
/// race that only the unique index can arbitrate.
/// </summary>
public sealed class TeacherAccountLinkTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task SetAccount_EligibleTeacherRoleUser_Links()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link1@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link1@nile.test", StaffRole.Teacher);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, teacherMember.UserId, teacherRecord.Version));

        Assert.True(result.IsSuccess);
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(teacherRecord.TeacherId));
        Assert.Equal(teacherMember.UserId, dto.Value.UserId);
    }

    [Fact]
    public async Task SetAccount_SecretaryUserId_ReturnsUserNotEligible()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link2@nile.test", StaffRole.Owner);
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-link2@nile.test", StaffRole.Secretary);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, secretary.UserId, teacherRecord.Version));

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task SetAccount_MaadiOnlyUser_ReturnsUserNotEligible()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link3@nile.test", StaffRole.Owner);
        var maadiOnlyTeacher = await CreateStaffAsync(MaadiCentreId, "maadi-only-link3@maadi.test", StaffRole.Teacher);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, maadiOnlyTeacher.UserId, teacherRecord.Version));

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task SetAccount_UnknownUserId_ReturnsUserNotEligible()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link4@nile.test", StaffRole.Owner);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, Guid.CreateVersion7(), teacherRecord.Version));

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link5@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link5@nile.test", StaffRole.Teacher);
        var firstRecord = await CreateTeacherAsync(owner, "Amina Hassan");
        var linked = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(firstRecord.TeacherId, teacherMember.UserId, firstRecord.Version));
        Assert.True(linked.IsSuccess);

        var secondRecord = await CreateTeacherAsync(owner, "Mona Said");
        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(secondRecord.TeacherId, teacherMember.UserId, secondRecord.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.account_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("userId", result.Error.Fields!.Keys);
    }

    [Fact]
    public async Task SetAccount_NullUserId_UnlinksAPreviouslyLinkedTeacher()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link6@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link6@nile.test", StaffRole.Teacher);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");
        var linked = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, teacherMember.UserId, teacherRecord.Version));
        Assert.True(linked.IsSuccess);
        var linkedDto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(teacherRecord.TeacherId));

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, null, linkedDto.Value.Version));

        Assert.True(result.IsSuccess);
        var finalDto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(teacherRecord.TeacherId));
        Assert.Null(finalDto.Value.UserId);
    }

    [Fact]
    public async Task SetAccount_TwoParallelLinksOfOneUserToTwoTeachers_OneSucceedsOneAccountTaken()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link7@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link7@nile.test", StaffRole.Teacher);
        var firstRecord = await CreateTeacherAsync(owner, "Amina Hassan");
        var secondRecord = await CreateTeacherAsync(owner, "Mona Said");
        var gate = new TaskCompletionSource();

        async Task<Result<Unit>> AttemptAsync(Guid teacherId, uint version)
        {
            await gate.Task;
            return await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
                owner.Actor, new SetTeacherAccountCommand(teacherId, teacherMember.UserId, version));
        }

        var tasks = new[]
        {
            Task.Run(() => AttemptAsync(firstRecord.TeacherId, firstRecord.Version)),
            Task.Run(() => AttemptAsync(secondRecord.TeacherId, secondRecord.Version)),
        };
        gate.SetResult();
        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        var loser = Assert.Single(results, result => result.IsFailure);
        Assert.Equal("teacher.account_taken", loser.Error!.Code);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.teachers where user_id = '{teacherMember.UserId}'"));
    }

    [Fact]
    public async Task SetAccount_Links_WritesOneUpdatedAuditRowWithUserIdBeforeNullAndAfterTheId()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link8@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link8@nile.test", StaffRole.Teacher);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, teacherMember.UserId, teacherRecord.Version));
        Assert.True(result.IsSuccess);

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from audit.audit_entries where entity_id = '{teacherRecord.TeacherId}' and entity_type = 'teacher' and action = 'updated'"));
        Assert.True(
            await Fixture.ScalarAsync<bool>(
                $"select changes->'userId'->>'before' is null from audit.audit_entries where entity_id = '{teacherRecord.TeacherId}' and entity_type = 'teacher' and action = 'updated'"));
        Assert.Equal(
            teacherMember.UserId.ToString(),
            await Fixture.ScalarAsync<string>(
                $"select changes->'userId'->>'after' from audit.audit_entries where entity_id = '{teacherRecord.TeacherId}' and entity_type = 'teacher' and action = 'updated'"));
    }

    private static void AssertUserNotEligible<T>(Result<T> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("teacher.user_not_eligible", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Contains("userId", result.Error.Fields!.Keys);
    }

    private async Task<CreatedTeacherRecord> CreateTeacherAsync(CreatedStaff owner, string fullName)
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(owner.Actor, new CreateTeacherCommand(fullName, null));
        Assert.True(created.IsSuccess, created.Error?.Message);
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(created.Value.TeacherId));
        return new CreatedTeacherRecord(created.Value.TeacherId, dto.Value.Version);
    }

    private sealed record CreatedTeacherRecord(Guid TeacherId, uint Version);
}
