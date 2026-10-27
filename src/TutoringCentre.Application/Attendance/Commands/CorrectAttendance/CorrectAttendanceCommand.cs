using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Commands.CorrectAttendance;

public sealed record CorrectAttendanceCommand(Guid RecordId, AttendanceStatus Status, string Reason, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceCorrect;
}
