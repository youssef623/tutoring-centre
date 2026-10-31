using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Attendance.Queries.GetSessionAttendance;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetSessionAttendanceHandler(IAttendanceRosterSource roster, ITeachingScope teachingScope,
    IAttendanceReadService reads, ICentreTimeZoneProvider timeZones, IClock clock, ICurrentActor actor)
    : IQueryHandler<GetSessionAttendanceQuery, SessionAttendanceDto>
{
    public async Task<Result<SessionAttendanceDto>> HandleAsync(GetSessionAttendanceQuery query, CancellationToken cancellationToken)
    {
        if (actor.Actor is IPortalActor)
            return Result<SessionAttendanceDto>.Failure(Error.Forbidden("attendance.not_available_to_portal", "Attendance rosters are not available to portal users."));
        var session = await roster.GetSessionAsync(query.SessionId, cancellationToken);
        if (session is null || !(await teachingScope.GetAsync(cancellationToken)).Allows(session.TeacherId))
        {
            return Result<SessionAttendanceDto>.Failure(AttendanceErrors.SessionNotFound());
        }

        var expected = await roster.GetExpectedStudentsAsync(session.SessionId, cancellationToken);
        var records = (await reads.ListForSessionAsync(session.SessionId, cancellationToken)).ToDictionary(row => row.StudentId, row => row.Record);
        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        return Result<SessionAttendanceDto>.Success(new(
            new(session.SessionId, session.GroupName, session.SubjectName, session.TeacherName, session.StartsAt,
                session.EndsAt, session.LocalDate, session.IsCancelled ? "cancelled" : "scheduled"), zone.Id,
            AttendanceView.Window(session.StartsAt, session.EndsAt, session.IsCancelled, clock.UtcNow),
            expected.Select(student => new AttendanceStudentDto(student.StudentId, student.FullName, records.GetValueOrDefault(student.StudentId))).ToArray()));
    }
}
