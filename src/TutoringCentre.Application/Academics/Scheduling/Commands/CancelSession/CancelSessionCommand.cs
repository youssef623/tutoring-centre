using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;

/// <summary>Intent to cancel one future session (a holiday, a sick teacher), guarded by the version the client last read.</summary>
public sealed record CancelSessionCommand(Guid SessionId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
