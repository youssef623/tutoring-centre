using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: tenancy in both directions. Every ID in an enrollment command is resolved inside the actor's own
/// centre, so another centre's group, student or enrollment answers the same not-found as an unknown ID — never
/// a foreign-key 500 from the composite keys behind them — and the other centre's rows are unchanged.
/// </summary>
public sealed class EnrollmentTenantIsolationTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    public static TheoryData<bool> Directions => new() { true, false };

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Enroll_IntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        var result = await EnrollAsync(own.Secretary.Actor, foreign.GroupId, own.FreeStudentId);

        await AssertRefusedAsync(result, "group.not_found", foreign);
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Enroll_TheOtherCentresStudentIntoAnOwnGroup_ReturnsStudentNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        var result = await EnrollAsync(own.Secretary.Actor, own.GroupId, foreign.FreeStudentId);

        await AssertRefusedAsync(result, "student.not_found", foreign);
        Assert.Equal(0, await Fixture.ScalarAsync<long>($"select count(*) from academics.enrollments where student_id = '{foreign.FreeStudentId}'"));
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Withdraw_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        // Version 0 is deliberately meaningless: the lookup must refuse before any version comparison happens.
        var result = await Fixture.SendAsAsync<WithdrawEnrollmentCommand, Unit>(
            own.Secretary.Actor, new WithdrawEnrollmentCommand(foreign.EnrollmentId, 0));

        await AssertRefusedAsync(result, "enrollment.not_found", foreign);
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Transfer_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        var result = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            own.Secretary.Actor, new TransferEnrollmentCommand(foreign.EnrollmentId, own.GroupId, 0));

        await AssertRefusedAsync(result, "enrollment.not_found", foreign);
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Transfer_AnOwnEnrollmentIntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);
        var version = (await GetStudentEnrollmentAsync(own.Secretary.Actor, own.EnrolledStudentId, own.EnrollmentId)).Version;

        var result = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            own.Secretary.Actor, new TransferEnrollmentCommand(own.EnrollmentId, foreign.GroupId, version));

        await AssertRefusedAsync(result, "group.not_found", foreign);
        Assert.Equal("active", await Fixture.ScalarAsync<string>($"select status from academics.enrollments where id = '{own.EnrollmentId}'"));
    }

    private async Task AssertRefusedAsync(Result result, string code, CentreWorld foreign)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
        Assert.Equal(foreign.Snapshot, await SnapshotAsync(foreign.CentreId));
    }

    private static (CentreWorld Own, CentreWorld Foreign) Pick((CentreWorld Nile, CentreWorld Maadi) world, bool nileActsOnMaadi) =>
        nileActsOnMaadi ? (world.Nile, world.Maadi) : (world.Maadi, world.Nile);

    /// <summary>Every enrollment and group row of one centre: any change in the other centre shows up.</summary>
    private Task<string> SnapshotAsync(Guid centreId) =>
        Fixture.ScalarAsync<string>(
            $"""
            select concat(
                (select coalesce(string_agg(concat_ws('|', id, group_id, student_id, status, xmin), ',' order by id), '')
                 from academics.enrollments where centre_id = '{centreId}'),
                '#',
                (select coalesce(string_agg(concat_ws('|', id, capacity, status, xmin), ',' order by id), '')
                 from academics.groups where centre_id = '{centreId}'))
            """);

    private async Task<(CentreWorld Nile, CentreWorld Maadi)> ArrangeAsync()
    {
        var nile = await ArrangeCentreAsync(NileCentreId, "nile");
        var maadi = await ArrangeCentreAsync(MaadiCentreId, "maadi");
        return (nile with { Snapshot = await SnapshotAsync(NileCentreId) }, maadi with { Snapshot = await SnapshotAsync(MaadiCentreId) });
    }

    private async Task<CentreWorld> ArrangeCentreAsync(Guid centreId, string tag)
    {
        var secretary = await CreateStaffAsync(centreId, $"secretary-iso@{tag}.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(centreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(centreId, "Amina Hassan");
        var groupId = await Fixture.SeedGroupAsync(centreId, "Grade 10 A", subjectId, teacherId);
        var students = await SeedStudentsAsync(centreId, 2, $"Student {tag}");
        var enrollmentId = await EnrollOkAsync(secretary.Actor, groupId, students[0]);
        return new CentreWorld(centreId, secretary, groupId, students[0], students[1], enrollmentId, string.Empty);
    }

    private sealed record CentreWorld(
        Guid CentreId,
        CreatedStaff Secretary,
        Guid GroupId,
        Guid EnrolledStudentId,
        Guid FreeStudentId,
        Guid EnrollmentId,
        string Snapshot);
}
