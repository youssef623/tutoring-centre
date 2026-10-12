using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Queries.GetParent;

/// <summary>Used by the parent detail page. No centre: scope comes from the actor.</summary>
public sealed record GetParentQuery(Guid ParentId) : IQuery<ParentDetailDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
