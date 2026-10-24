using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;

/// <summary>Intent to stop a rule after <paramref name="LastDate"/>, guarded by the version the client last read.</summary>
public sealed record EndScheduleRuleCommand(Guid ScheduleRuleId, DateOnly LastDate, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
