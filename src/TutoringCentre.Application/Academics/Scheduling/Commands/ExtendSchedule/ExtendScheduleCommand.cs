using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;

/// <summary>
/// Intent to generate the acting centre's sessions for every active rule through <paramref name="Through"/>. One
/// centre per command: a nightly job dispatches it once per centre as that centre's system actor.
/// </summary>
public sealed record ExtendScheduleCommand(DateOnly Through) : ICommand<ExtendScheduleResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
