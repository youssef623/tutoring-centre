using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Queries.GetGroup;

/// <summary>One group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.</summary>
public sealed record GetGroupQuery(Guid GroupId) : IQuery<GroupDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsView;
}
