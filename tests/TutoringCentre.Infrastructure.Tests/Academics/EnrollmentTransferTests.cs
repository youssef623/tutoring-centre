using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: a transfer is one transaction that ends the old enrollment and starts a new one. It moves exactly
/// one seat, changes nothing when it fails, and — because it locks only the target — two opposite transfers in
/// parallel both complete instead of waiting on each other in a cycle.
/// </summary>
public sealed class EnrollmentTransferTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    private const int Repetitions = 20;

    [Fact]
    public async Task Transfer_MovesTheSeatFromTheSourceToTheTarget()
    {
        var world = await ArrangeAsync("tr1");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[0]);
        await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[1]);

        var moved = await TransferAsync(world, enrollmentId, world.TargetId);

        Assert.True(moved.IsSuccess, moved.Error?.Message);
        Assert.Equal(1, (await GetGroupAsync(world.Secretary.Actor, world.SourceId)).EnrolledCount);
        Assert.Equal(1, (await GetGroupAsync(world.Secretary.Actor, world.TargetId)).EnrolledCount);
        Assert.Equal(
            $"ended|transferred|{world.SourceId}",
            await Fixture.ScalarAsync<string>($"select concat_ws('|', status, end_reason, group_id) from academics.enrollments where id = '{enrollmentId}'"));
        Assert.Equal(
            $"active|{world.TargetId}|{world.Students[0]}",
            await Fixture.ScalarAsync<string>(
                $"select concat_ws('|', status, group_id, student_id) from academics.enrollments where id = '{moved.Value.EnrollmentId}'"));
    }

    [Fact]
    public async Task Transfer_IntoAFullTarget_ReturnsGroupFullAndLeavesBothGroupsUnchanged()
    {
        var world = await ArrangeAsync("tr2", targetCapacity: 1);
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[0]);
        await EnrollOkAsync(world.Secretary.Actor, world.TargetId, world.Students[1]);
        var before = await SnapshotAsync(world);

        var moved = await TransferAsync(world, enrollmentId, world.TargetId);

        Assert.Equal("group.full", moved.Error!.Code);
        Assert.Equal(before, await SnapshotAsync(world));
        Assert.Equal(
            "active",
            await Fixture.ScalarAsync<string>($"select status from academics.enrollments where id = '{enrollmentId}'"));
    }

    [Fact]
    public async Task Transfer_WithAStaleVersion_ReturnsConcurrencyStaleAndChangesNothing()
    {
        var world = await ArrangeAsync("tr3");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[0]);
        var version = (await GetStudentEnrollmentAsync(world.Secretary.Actor, world.Students[0], enrollmentId)).Version;
        var before = await SnapshotAsync(world);

        var moved = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            world.Secretary.Actor, new TransferEnrollmentCommand(enrollmentId, world.TargetId, version + 1));

        // The new row was inserted in the same save that found the old one stale, so the rollback removes both.
        Assert.Equal("concurrency.stale", moved.Error!.Code);
        Assert.Equal(before, await SnapshotAsync(world));
    }

    [Fact]
    public async Task TwoOppositeTransfersInParallel_BothCompleteWithoutDeadlock_AcrossTwentyRepetitions()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-opposite@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            var groupA = await Fixture.SeedGroupAsync(NileCentreId, $"Opposite A {iteration:D2}", subjectId, teacherId);
            var groupB = await Fixture.SeedGroupAsync(NileCentreId, $"Opposite B {iteration:D2}", subjectId, teacherId);
            var students = await SeedStudentsAsync(NileCentreId, 2, $"Opposite {iteration:D2}");
            var inA = await EnrollOkAsync(secretary.Actor, groupA, students[0]);
            var inB = await EnrollOkAsync(secretary.Actor, groupB, students[1]);
            var versionA = (await GetStudentEnrollmentAsync(secretary.Actor, students[0], inA)).Version;
            var versionB = (await GetStudentEnrollmentAsync(secretary.Actor, students[1], inB)).Version;

            var results = await RunTogetherAsync(new Func<Task<Result<TransferEnrollmentResult>>>[]
            {
                () => Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
                    secretary.Actor, new TransferEnrollmentCommand(inA, groupB, versionA)),
                () => Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
                    secretary.Actor, new TransferEnrollmentCommand(inB, groupA, versionB)),
            });

            Assert.All(results, result => Assert.True(result.IsSuccess, result.Error?.Message));
            Assert.Equal(1, await ActiveRowsAsync(groupA));
            Assert.Equal(1, await ActiveRowsAsync(groupB));
            Assert.Equal(
                students[1],
                await Fixture.ScalarAsync<Guid>($"select student_id from academics.enrollments where group_id = '{groupA}' and status = 'active'"));
        }
    }

    private async Task<Result<TransferEnrollmentResult>> TransferAsync(World world, Guid enrollmentId, Guid targetId)
    {
        var studentId = await Fixture.ScalarAsync<Guid>($"select student_id from academics.enrollments where id = '{enrollmentId}'");
        var version = (await GetStudentEnrollmentAsync(world.Secretary.Actor, studentId, enrollmentId)).Version;
        return await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            world.Secretary.Actor, new TransferEnrollmentCommand(enrollmentId, targetId, version));
    }

    /// <summary>Every enrollment row of both groups, in a stable order: any change, insert or end shows up.</summary>
    private Task<string> SnapshotAsync(World world) =>
        Fixture.ScalarAsync<string>(
            $"""
            select coalesce(string_agg(concat_ws('|', id, group_id, status, end_reason, xmin), ',' order by id), '')
            from academics.enrollments where group_id in ('{world.SourceId}', '{world.TargetId}')
            """);

    private async Task<World> ArrangeAsync(string tag, int targetCapacity = 20)
    {
        var secretary = await CreateStaffAsync(NileCentreId, $"secretary-{tag}@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");
        var sourceId = await CreateGroupAsync(secretary.Actor, "Grade 10 A", subjectId, teacherId);
        var targetId = await CreateGroupAsync(secretary.Actor, "Grade 10 B", subjectId, teacherId, targetCapacity);
        var students = await SeedStudentsAsync(NileCentreId, 2, $"Student {tag}");
        return new World(secretary, sourceId, targetId, students);
    }

    private sealed record World(CreatedStaff Secretary, Guid SourceId, Guid TargetId, IReadOnlyList<Guid> Students);
}
