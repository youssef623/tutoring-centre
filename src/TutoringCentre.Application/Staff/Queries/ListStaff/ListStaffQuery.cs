using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Queries.ListStaff;

/// <summary>The list behind the staff page. No centre, no parameters: scope comes from the actor.</summary>
public sealed record ListStaffQuery : IQuery<IReadOnlyList<StaffMemberDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffView;
}
