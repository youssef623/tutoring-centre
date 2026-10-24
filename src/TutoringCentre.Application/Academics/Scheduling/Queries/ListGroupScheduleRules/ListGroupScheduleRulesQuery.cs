using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;

/// <summary>The weekly rules of one group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.</summary>
public sealed record ListGroupScheduleRulesQuery(Guid GroupId)
    : IQuery<IReadOnlyList<ScheduleRuleDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleView;
}
