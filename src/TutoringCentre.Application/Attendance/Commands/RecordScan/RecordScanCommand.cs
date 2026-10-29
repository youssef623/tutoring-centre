using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Commands.RecordScan;

/// <summary>The scanner supplies its credential as body data, never as a URL parameter.</summary>
public sealed record RecordScanCommand(Guid SessionId, string Token)
    : ICommand<ScanResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceMark;

    public override string ToString() => $"RecordScanCommand {{ SessionId = {SessionId} }}";
}
