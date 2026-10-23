using Npgsql;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10, the schedule's Week 8 demo: the capacity lock (ADR 0011) under real parallel transactions against
/// real PostgreSQL, each attempt in its own scope and connection, repeated because a race test that runs once can
/// pass by luck. Every count is read through the superuser connection, not inferred from the returned results.
/// A loser is a valid request that lost the race, so it gets the Domain's own answer, never an exception.
/// </summary>
public sealed class LastSeatConcurrencyTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    private const int Repetitions = 20;
    private const int LastSeatContenders = 10;
    private const int SingleSeatContenders = 50;
    private const int SingleSeatRepetitions = 5;

    [Fact]
    public void AppConnectionPool_IsNotSmallerThanTheLargestParallelism()
    {
        // A pool smaller than the parallelism would queue attempts in the client, not at the row lock, and the
        // races below would stop testing the database.
        Assert.True(new NpgsqlConnectionStringBuilder(Fixture.AppConnectionString).MaxPoolSize >= SingleSeatContenders);
    }

    [Fact]
    public async Task TenParallelEnrollmentsIntoTheLastSeat_ExactlyOneSucceedsAndNineGetGroupFull_AcrossTwentyRepetitions()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-last-seat@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            // A fresh group per repetition, so no seat taken in one repetition counts toward the next.
            var groupId = await Fixture.SeedGroupAsync(NileCentreId, $"Last Seat {iteration:D2}", subjectId, teacherId, capacity: 20);
            await SeedEnrollmentsAsync(NileCentreId, groupId, await SeedStudentsAsync(NileCentreId, 19, $"Seated {iteration:D2}"));
            var contenders = await SeedStudentsAsync(NileCentreId, LastSeatContenders, $"Contender {iteration:D2}");

            var results = await RunTogetherAsync(contenders.Select(studentId =>
                (Func<Task<Result<EnrollStudentResult>>>)(() => EnrollAsync(secretary.Actor, groupId, studentId))));

            Assert.Equal(1, results.Count(result => result.IsSuccess));
            Assert.Equal(LastSeatContenders - 1, results.Count(result => result.IsFailure && result.Error!.Code == "group.full"));
            Assert.Equal(20, await ActiveRowsAsync(groupId));

            // The nine losers rolled back, audit rows included: only the winner's enrollment was audited.
            Assert.Equal(
                1,
                await Fixture.ScalarAsync<long>(
                    $"select count(*) from audit.audit_entries where entity_type = 'enrollment' and changes->'groupId'->>'after' = '{groupId}'"));
        }
    }

    [Fact]
    public async Task FiftyParallelEnrollmentsIntoASingleSeatGroup_ExactlyOneSucceeds()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-single-seat@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < SingleSeatRepetitions; iteration++)
        {
            var groupId = await Fixture.SeedGroupAsync(NileCentreId, $"Single Seat {iteration:D2}", subjectId, teacherId, capacity: 1);
            var contenders = await SeedStudentsAsync(NileCentreId, SingleSeatContenders, $"Single {iteration:D2}");

            var results = await RunTogetherAsync(contenders.Select(studentId =>
                (Func<Task<Result<EnrollStudentResult>>>)(() => EnrollAsync(secretary.Actor, groupId, studentId))));

            Assert.Equal(1, results.Count(result => result.IsSuccess));
            Assert.All(results.Where(result => result.IsFailure), result => Assert.Equal("group.full", result.Error!.Code));
            Assert.Equal(1, await ActiveRowsAsync(groupId));
        }
    }

    [Fact]
    public async Task TheSameStudentEnrolledTwiceInParallel_OneSucceedsAndTheOtherGetsAlreadyEnrolled()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-same-student@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            var groupId = await Fixture.SeedGroupAsync(NileCentreId, $"Twice {iteration:D2}", subjectId, teacherId);
            var studentId = (await SeedStudentsAsync(NileCentreId, 1, $"Twice {iteration:D2}"))[0];

            var results = await RunTogetherAsync(Enumerable.Range(0, 2).Select(_ =>
                (Func<Task<Result<EnrollStudentResult>>>)(() => EnrollAsync(secretary.Actor, groupId, studentId))));

            Assert.Equal(1, results.Count(result => result.IsSuccess));
            // From the handler's check (it waited at the lock, then saw the winner's row) or, failing that, the
            // partial unique index: both answer the same code.
            Assert.Equal("enrollment.already_enrolled", Assert.Single(results, result => result.IsFailure).Error!.Code);
            Assert.Equal(1, await ActiveRowsAsync(groupId));
        }
    }

    [Fact]
    public async Task EnrollAndShrinkInParallel_NeverLeaveMoreStudentsThanSeats()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-shrink@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            var groupId = await Fixture.SeedGroupAsync(NileCentreId, $"Shrink {iteration:D2}", subjectId, teacherId, capacity: 20);
            await SeedEnrollmentsAsync(NileCentreId, groupId, await SeedStudentsAsync(NileCentreId, 19, $"Shrink Seated {iteration:D2}"));
            var latecomer = (await SeedStudentsAsync(NileCentreId, 1, $"Shrink Latecomer {iteration:D2}"))[0];
            var version = (await GetGroupAsync(secretary.Actor, groupId)).Version;

            var results = await RunTogetherAsync(new Func<Task<Result>>[]
            {
                async () => await EnrollAsync(secretary.Actor, groupId, latecomer),
                async () => await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(
                    secretary.Actor, new ChangeGroupCapacityCommand(groupId, 19, version)),
            });
            var (enroll, shrink) = (results[0], results[1]);

            if (enroll.IsSuccess)
            {
                Assert.Equal("group.capacity_below_enrolled", shrink.Error?.Code);
                Assert.Equal(20, await CapacityAsync(groupId));
                Assert.Equal(20, await ActiveRowsAsync(groupId));
            }
            else
            {
                Assert.Equal("group.full", enroll.Error!.Code);
                Assert.True(shrink.IsSuccess, shrink.Error?.Message);
                Assert.Equal(19, await CapacityAsync(groupId));
                Assert.Equal(19, await ActiveRowsAsync(groupId));
            }

            Assert.True(await ActiveRowsAsync(groupId) <= await CapacityAsync(groupId));
        }
    }
}
