using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using CorrectCommand = TutoringCentre.Application.Attendance.Commands.CorrectAttendance.CorrectAttendanceCommand;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Academics;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public abstract class AttendanceCommandTestBase(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private protected async Task<Scenario> ArrangeAttendanceAsync(Guid centreId, string tag)
    {
        var world = await ArrangeWorldAsync(centreId, tag);
        var rule = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var sessionId = await Fixture.ScalarAsync<Guid>($"select id from academics.class_sessions where schedule_rule_id = '{rule.ScheduleRuleId}' order by starts_at limit 1");
        var studentId = await Fixture.SeedStudentAsync(centreId, "Attendance Student");
        var enrolled = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(world.Owner.Actor, new(world.G1, studentId));
        Assert.True(enrolled.IsSuccess, enrolled.Error?.Message);
        var facts = await SessionFactsAsync(world.Owner.Actor, sessionId);
        Assert.NotNull(facts);
        return new(world.Owner.Actor, world.T1, world.T2, world.G1, sessionId, studentId,
            enrolled.Value.EnrollmentId, facts.StartsAt, facts.EndsAt);
    }

    private protected async Task<StaffActor> LinkTeacherAsync(Scenario scenario, Guid teacherId, string email)
    {
        var teacher = await CreateStaffAsync(scenario.Owner.CentreId!.Value, email, StaffRole.Teacher);
        var result = await Clocked.SendAsAsync<SetTeacherAccountCommand, Unit>(scenario.Owner,
            new(teacherId, teacher.UserId, await TeacherVersionAsync(scenario.Owner, teacherId)));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return teacher.Actor;
    }

    private protected Task<Result<MarkAttendanceResult>> MarkAsync(Actor actor, Scenario scenario, AttendanceStatus status = AttendanceStatus.Present) =>
        Clocked.SendAsAsync<MarkAttendanceCommand, MarkAttendanceResult>(actor,
            new(scenario.SessionId, [new(scenario.StudentId, status)]));

    private protected Task<Result<Unit>> CorrectAsync(Actor actor, Guid recordId, uint version, AttendanceStatus status = AttendanceStatus.Excused) =>
        Clocked.SendAsAsync<CorrectCommand, Unit>(actor, new(recordId, status, "  parent   called ", version));

    private protected Task<Guid> RecordIdAsync(Scenario scenario) => Fixture.ScalarAsync<Guid>(
        $"select id from attendance.attendance_records where session_id = '{scenario.SessionId}' and student_id = '{scenario.StudentId}'");

    private protected async Task<uint> RecordVersionAsync(Guid id) => checked((uint)await Fixture.ScalarAsync<long>(
        $"select xmin::text::bigint from attendance.attendance_records where id = '{id}'"));

    private protected async Task<AttendanceSessionFacts?> SessionFactsAsync(Actor actor, Guid id)
    {
        await using var scope = Clocked.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(readOnly: true, CancellationToken.None);
        var result = await scope.ServiceProvider.GetRequiredService<IAttendanceRosterSource>().GetSessionAsync(id, CancellationToken.None);
        await unit.CommitAsync(CancellationToken.None);
        return result;
    }

    private protected async Task ReassignSessionAsync(Scenario scenario, Guid teacherId)
    {
        await using var scope = Clocked.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(scenario.Owner);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(readOnly: false, CancellationToken.None);
        var version = checked((uint)await Fixture.ScalarAsync<long>($"select xmin::text::bigint from academics.class_sessions where id = '{scenario.SessionId}'"));
        var session = await scope.ServiceProvider.GetRequiredService<IClassSessionRepository>()
            .GetByIdAsync(scenario.SessionId, version, CancellationToken.None);
        Assert.NotNull(session);
        Assert.True(session.ReassignTeacher(teacherId, Time.GetUtcNow()).IsSuccess);
        var saved = await unit.SaveChangesAsync(CancellationToken.None);
        Assert.True(saved.IsSuccess, saved.Error?.Message);
        await unit.CommitAsync(CancellationToken.None);
    }

    private protected sealed record Scenario(StaffActor Owner, Guid TeacherId, Guid SubstituteId, Guid GroupId,
        Guid SessionId, Guid StudentId, Guid EnrollmentId, DateTimeOffset StartsAt, DateTimeOffset EndsAt);
}
