using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance.Commands.CorrectAttendance;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Attendance;

internal sealed class AttendanceHandlerFixture
{
    public static Guid TeacherId { get; } = new("11111111-1111-7111-8111-111111111111");

    public AttendanceHandlerFixture()
    {
        Actor.Actor = Staff;
        Session = new(Guid.NewGuid(), Guid.NewGuid(), "Group", "Subject", TeacherId, "Teacher",
            new(2026, 12, 5, 16, 0, 0, TimeSpan.Zero), new(2026, 12, 5, 17, 30, 0, TimeSpan.Zero), new(2026, 12, 5), null, false);
        OtherSession = Session with { SessionId = Guid.NewGuid(), TeacherId = Guid.NewGuid() };
        Roster.Sessions[Session.SessionId] = Session;
        Roster.Sessions[OtherSession.SessionId] = OtherSession;
        Scope.Scope = TeachingScope.Own(Session.TeacherId);
        Students = Enumerable.Range(0, 3).Select(index => new ExpectedStudent(Guid.NewGuid(), Guid.NewGuid(), $"Student {index}")).ToArray();
        Roster.Students[Session.SessionId] = Students;
        Clock.UtcNow = Session.StartsAt;
        MarkHandler = new(Roster, Scope, Records, Actor, Clock);
        CorrectHandler = new(Records, Roster, Scope, Actor, Clock);
    }

    public StaffActor Staff { get; } = new(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Secretary);
    public FakeCurrentActor Actor { get; } = new();
    public FakeClock Clock { get; } = new();
    public FakeAttendanceRosterSource Roster { get; } = new();
    public FakeTeachingScope Scope { get; } = new();
    public FakeAttendanceRecordRepository Records { get; } = new();
    public FakeUnitOfWork UnitOfWork { get; } = new();
    public AttendanceSessionFacts Session { get; }
    public AttendanceSessionFacts OtherSession { get; }
    public IReadOnlyList<ExpectedStudent> Students { get; }
    public MarkAttendanceHandler MarkHandler { get; }
    public CorrectAttendanceHandler CorrectHandler { get; }

    public MarkAttendanceCommand Marks() => new(Session.SessionId,
        Students.Select(student => new AttendanceMark(student.StudentId, AttendanceStatus.Present)).ToArray());

    public AttendanceRecord AddExisting(int index = 0)
    {
        var student = Students[index];
        var record = AttendanceRecord.Create(Staff.CentreId!.Value, Session.SessionId, student.StudentId, student.EnrollmentId,
            AttendanceStatus.Present, AttendanceSource.Manual, Staff.UserId,
            new(Session.StartsAt, Session.EndsAt, false), Session.StartsAt).Value;
        Records.Existing.Add(record);
        return record;
    }

    public async Task<Result<T>> DispatchAsync<TCommand, T>(Actor actor, TCommand command) where TCommand : ICommand<T>
    {
        Actor.Actor = actor;
        using var provider = new ServiceCollection()
            .AddSingleton<ICurrentActor>(Actor)
            .AddSingleton<IValidator<MarkAttendanceCommand>>(new MarkAttendanceValidator())
            .AddSingleton<IValidator<CorrectAttendanceCommand>>(new CorrectAttendanceValidator())
            .AddSingleton<ICommandHandler<MarkAttendanceCommand, MarkAttendanceResult>>(MarkHandler)
            .AddSingleton<ICommandHandler<CorrectAttendanceCommand, Unit>>(CorrectHandler)
            .BuildServiceProvider();
        return await new Dispatcher(provider, UnitOfWork, NullLogger<Dispatcher>.Instance)
            .SendAsync<TCommand, T>(command, CancellationToken.None);
    }
}
