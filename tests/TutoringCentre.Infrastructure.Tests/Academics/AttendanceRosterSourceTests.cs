using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

public sealed class AttendanceRosterSourceTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    [Fact]
    public async Task CardStudent_ReportsArchivedState()
    {
        var (actor, _, _) = await ArrangeSessionAsync("card-archive");
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Archived Student");
        await using var connection = new Npgsql.NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand($"update academics.students set status = 'archived' where id = '{student}'", connection);
        await command.ExecuteNonQueryAsync();
        Assert.Equal(new CardStudent(student, "Archived Student", true),
            await ReadAsync(actor, source => source.GetCardStudentAsync(student, CancellationToken.None)));
    }

    [Fact]
    public async Task CardGroup_ExcludesEndedAndFutureEnrollments()
    {
        var (actor, group, _) = await ArrangeSessionAsync("card-members");
        var active = await Fixture.SeedStudentAsync(NileCentreId, "Active Student");
        var ended = await Fixture.SeedStudentAsync(NileCentreId, "Ended Student");
        await EnrollAtClockAsync(actor, group, active);
        var enrollment = await EnrollAtClockAsync(actor, group, ended);
        Time.Advance(TimeSpan.FromMinutes(1));
        Assert.True((await Clocked.SendAsAsync<WithdrawEnrollmentCommand, Unit>(actor, new(enrollment, await VersionAsync(enrollment)))).IsSuccess);
        var current = await ReadAsync(actor, source => source.GetCardGroupAsync(group, CancellationToken.None));
        Assert.Equal(active, Assert.Single(current!.Students).StudentId);
        await using var connection = new Npgsql.NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand($"update academics.enrollments set enrolled_at = '2027-03-02T08:00Z' where student_id = '{active}'", connection);
        await command.ExecuteNonQueryAsync();
        Assert.Empty((await ReadAsync(actor, source => source.GetCardGroupAsync(group, CancellationToken.None)))!.Students);
    }

    [Fact]
    public async Task CardFacts_UnknownOrForeign_ReturnNothing()
    {
        var (actor, _, _) = await ArrangeSessionAsync("card-scope");
        var foreign = await ArrangeWorldAsync(MaadiCentreId, "card-foreign");
        var student = await Fixture.SeedStudentAsync(MaadiCentreId, "Foreign Student");
        foreach (var id in new[] { student, Guid.NewGuid() })
            Assert.Null(await ReadAsync(actor, source => source.GetCardStudentAsync(id, CancellationToken.None)));
        foreach (var id in new[] { foreign.G1, Guid.NewGuid() })
            Assert.Null(await ReadAsync(actor, source => source.GetCardGroupAsync(id, CancellationToken.None)));
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public async Task EnrollmentAtSessionStart_UsesInclusiveStart(int seconds, bool expected)
    {
        var (actor, group, session) = await ArrangeSessionAsync("start");
        var facts = await ReadAsync(actor, source => source.GetSessionAsync(session, CancellationToken.None));
        Assert.NotNull(facts);
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Expected Student");
        Time.SetUtcNow(facts.StartsAt.AddSeconds(seconds));
        var enrollment = await EnrollAtClockAsync(actor, group, student);
        var roster = await ReadAsync(actor, source => source.GetExpectedStudentsAsync(session, CancellationToken.None));
        Assert.Equal(expected, roster.Any(row => row.StudentId == student && row.EnrollmentId == enrollment));
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public async Task WithdrawalAtSessionStart_UsesExclusiveEnd(int seconds, bool expected)
    {
        var (actor, group, session) = await ArrangeSessionAsync("end");
        var facts = await ReadAsync(actor, source => source.GetSessionAsync(session, CancellationToken.None));
        Assert.NotNull(facts);
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Withdrawn Student");
        var enrollment = await EnrollAtClockAsync(actor, group, student);
        Time.SetUtcNow(facts.StartsAt.AddSeconds(seconds));
        var result = await Clocked.SendAsAsync<WithdrawEnrollmentCommand, Unit>(actor, new(enrollment, await VersionAsync(enrollment)));
        Assert.True(result.IsSuccess, result.Error?.Message);
        var roster = await ReadAsync(actor, source => source.GetExpectedStudentsAsync(session, CancellationToken.None));
        Assert.Equal(expected, roster.Any(row => row.StudentId == student));
    }

    [Fact]
    public async Task Transfer_PreservesSourceHistory_AndUsesTargetForLaterSession()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "transfer-roster");
        var sourceRule = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var targetRule = await AddRuleOkAsync(world.Owner.Actor, world.G3, DayOfWeek.Saturday, SixPm);
        var sourceSession = await FirstSessionAsync(sourceRule.ScheduleRuleId);
        var targetLaterSession = await Fixture.ScalarAsync<Guid>($"select id from academics.class_sessions where schedule_rule_id = '{targetRule.ScheduleRuleId}' order by starts_at offset 1 limit 1");
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Transferred Student");
        var enrollment = await EnrollAtClockAsync(world.Owner.Actor, world.G1, student);
        var facts = await ReadAsync(world.Owner.Actor, source => source.GetSessionAsync(sourceSession, CancellationToken.None));
        Time.SetUtcNow(facts!.EndsAt.AddHours(1));
        var moved = await Clocked.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(world.Owner.Actor,
            new(enrollment, world.G3, await VersionAsync(enrollment)));
        Assert.True(moved.IsSuccess, moved.Error?.Message);
        var earlier = await ReadAsync(world.Owner.Actor, source => source.GetExpectedStudentsAsync(sourceSession, CancellationToken.None));
        var later = await ReadAsync(world.Owner.Actor, source => source.GetExpectedStudentsAsync(targetLaterSession, CancellationToken.None));
        Assert.Contains(earlier, row => row.StudentId == student && row.EnrollmentId == enrollment);
        Assert.Contains(later, row => row.StudentId == student && row.EnrollmentId == moved.Value.EnrollmentId);
    }

    [Fact]
    public async Task AnotherCentreSession_ReturnsNullAndEmptyRoster()
    {
        var world = await ArrangeWorldAsync(MaadiCentreId, "maadi-roster");
        var rule = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var session = await FirstSessionAsync(rule.ScheduleRuleId);
        var nile = (await CreateStaffAsync(NileCentreId, "nile-roster@example.test", TutoringCentre.Domain.Identity.StaffRole.Owner)).Actor;
        Assert.Null(await ReadAsync(nile, source => source.GetSessionAsync(session, CancellationToken.None)));
        Assert.Empty(await ReadAsync(nile, source => source.GetExpectedStudentsAsync(session, CancellationToken.None)));
        Assert.Null(await ReadAsync(nile, source => source.GetSessionAsync(Guid.NewGuid(), CancellationToken.None)));
    }

    [Fact]
    public async Task Roster_IsOrderedByNormalisedName_AndReturnsNoTrackedEntities()
    {
        var (actor, group, session) = await ArrangeSessionAsync("names");
        var zed = await Fixture.SeedStudentAsync(NileCentreId, "Zed Student");
        var amy = await Fixture.SeedStudentAsync(NileCentreId, "Amy Student");
        await EnrollAtClockAsync(actor, group, zed);
        await EnrollAtClockAsync(actor, group, amy);
        var roster = await ReadAsync(actor, source => source.GetExpectedStudentsAsync(session, CancellationToken.None));
        Assert.Equal(["Amy Student", "Zed Student"], roster.Select(row => row.FullName).ToArray());
    }

    [Fact]
    public async Task DateList_IncludesCancellation_AndFiltersBySessionTeacher()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "date-roster");
        var rule = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var session = await FirstSessionAsync(rule.ScheduleRuleId);
        // Change the group's current teacher alone, as persisted history may differ from future ownership.
        await using (var connection = new Npgsql.NpgsqlConnection(Fixture.SuperuserConnectionString))
        {
            await connection.OpenAsync();
            await using var command = new Npgsql.NpgsqlCommand($"update academics.groups set teacher_id = '{world.T2}' where id = '{world.G1}'; update academics.class_sessions set status = 'cancelled' where id = '{session}'", connection);
            await command.ExecuteNonQueryAsync();
        }

        var facts = await ReadAsync(world.Owner.Actor, source => source.GetSessionAsync(session, CancellationToken.None));
        Assert.Equal(world.T1, facts!.TeacherId);
        Assert.Equal("Amina Hassan", facts.TeacherName);
        var rows = await ReadAsync(world.Owner.Actor, source => source.ListSessionsOnDateAsync(facts.LocalDate, world.T1, CancellationToken.None));
        var listed = Assert.Single(rows);
        Assert.Equal(session, listed.SessionId);
        Assert.True(listed.IsCancelled);
        Assert.Empty(await ReadAsync(world.Owner.Actor, source => source.ListSessionsOnDateAsync(facts.LocalDate, world.T2, CancellationToken.None)));
        Assert.Empty(await ReadAsync(world.Owner.Actor, source => source.ListSessionsOnDateAsync(facts.LocalDate.AddDays(1), null, CancellationToken.None)));
    }

    private async Task<(StaffActor Actor, Guid Group, Guid Session)> ArrangeSessionAsync(string tag)
    {
        var world = await ArrangeWorldAsync(NileCentreId, tag);
        var rule = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        return (world.Owner.Actor, world.G1, await FirstSessionAsync(rule.ScheduleRuleId));
    }

    private Task<Guid> FirstSessionAsync(Guid rule) => Fixture.ScalarAsync<Guid>(
        $"select id from academics.class_sessions where schedule_rule_id = '{rule}' order by starts_at limit 1");

    private async Task<Guid> EnrollAtClockAsync(Actor actor, Guid group, Guid student)
    {
        var result = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(actor, new(group, student));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.EnrollmentId;
    }

    private async Task<uint> VersionAsync(Guid enrollment) => checked((uint)await Fixture.ScalarAsync<long>(
        $"select xmin::text::bigint from academics.enrollments where id = '{enrollment}'"));

    private async Task<T> ReadAsync<T>(Actor actor, Func<IAttendanceRosterSource, Task<T>> read)
    {
        await using var scope = Clocked.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(readOnly: true, CancellationToken.None);
        var result = await read(scope.ServiceProvider.GetRequiredService<IAttendanceRosterSource>());
        Assert.Empty(scope.ServiceProvider.GetRequiredService<AppDbContext>().ChangeTracker.Entries());
        await unit.CommitAsync(CancellationToken.None);
        return result;
    }
}
