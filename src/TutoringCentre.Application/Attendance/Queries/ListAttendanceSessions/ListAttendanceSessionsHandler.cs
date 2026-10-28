using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Queries.ListAttendanceSessions;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ListAttendanceSessionsHandler(IAttendanceRosterSource roster, ITeachingScope teachingScope,
    IAttendanceReadService reads, ICentreTimeZoneProvider timeZones, IClock clock)
    : IQueryHandler<ListAttendanceSessionsQuery, AttendanceSessionsDto>
{
    public async Task<Result<AttendanceSessionsDto>> HandleAsync(ListAttendanceSessionsQuery query, CancellationToken cancellationToken)
    {
        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var now = clock.UtcNow;
        var date = query.Date ?? DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(now, zone).DateTime);
        var scope = await teachingScope.GetAsync(cancellationToken);
        if (scope.Kind == TeachingScopeKind.None)
        {
            return Result<AttendanceSessionsDto>.Success(new(date, zone.Id, []));
        }

        var sessions = await roster.ListSessionsOnDateAsync(date, scope.TeacherId, cancellationToken);
        var counts = await reads.CountForSessionsAsync(sessions.Select(session => session.SessionId).ToArray(), cancellationToken);
        var items = new List<AttendanceSessionListItemDto>();
        foreach (var session in sessions)
        {
            var expected = await roster.GetExpectedStudentsAsync(session.SessionId, cancellationToken);
            items.Add(new(session.SessionId, session.GroupId, session.GroupName, session.SubjectName, session.TeacherName,
                session.StartsAt, session.EndsAt, session.Room, session.IsCancelled ? "cancelled" : "scheduled",
                AttendanceView.Window(session.StartsAt, session.EndsAt, session.IsCancelled, now), expected.Count,
                counts.GetValueOrDefault(session.SessionId)));
        }

        return Result<AttendanceSessionsDto>.Success(new(date, zone.Id, items));
    }
}
