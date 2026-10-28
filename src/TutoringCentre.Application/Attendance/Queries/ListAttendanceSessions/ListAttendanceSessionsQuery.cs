using FluentValidation;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Queries.ListAttendanceSessions;

public sealed record ListAttendanceSessionsQuery(DateOnly? Date = null) : IQuery<AttendanceSessionsDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceView;
}

internal sealed class ListAttendanceSessionsValidator : AbstractValidator<ListAttendanceSessionsQuery>
{
    public ListAttendanceSessionsValidator(ICentreTimeZoneProvider timeZones, IClock clock, ICurrentActor actor)
    {
        RuleFor(query => query.Date).MustAsync(async (date, ct) =>
        {
            if (date is null || actor.Actor.CentreId is null) { return true; }
            var zone = await timeZones.GetForCurrentCentreAsync(ct);
            var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.UtcNow, zone).DateTime);
            return Math.Abs(date.Value.DayNumber - today.DayNumber) <= 366;
        });
    }
}
