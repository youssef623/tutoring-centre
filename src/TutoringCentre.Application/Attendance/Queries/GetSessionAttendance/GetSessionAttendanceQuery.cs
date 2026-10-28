using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Queries.GetSessionAttendance;

public sealed record GetSessionAttendanceQuery(Guid SessionId) : IQuery<SessionAttendanceDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceView;
}
