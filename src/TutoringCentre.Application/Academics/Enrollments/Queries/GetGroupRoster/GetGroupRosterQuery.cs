using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;

/// <summary>Who holds a seat in one group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.</summary>
public sealed record GetGroupRosterQuery(Guid GroupId) : IQuery<GroupRosterDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsView;
}
