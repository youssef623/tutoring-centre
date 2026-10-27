using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Commands.MarkAttendance;

public sealed record AttendanceMark(Guid StudentId, AttendanceStatus Status);
public sealed record MarkAttendanceResult(int Created, int Updated, int Unchanged);

public sealed record MarkAttendanceCommand(Guid SessionId, IReadOnlyList<AttendanceMark> Marks)
    : ICommand<MarkAttendanceResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceMark;
}
