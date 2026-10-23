using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Queries.ListGroups;

/// <summary>
/// The groups list. No centre: tenancy comes from the actor. The filters are intersected with the actor's
/// teacher scope, so a scoped teacher filtering by another teacher gets an empty list, not an error.
/// </summary>
public sealed record ListGroupsQuery(Guid? SubjectId, Guid? TeacherId, bool IncludeArchived)
    : IQuery<IReadOnlyList<GroupDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsView;
}
