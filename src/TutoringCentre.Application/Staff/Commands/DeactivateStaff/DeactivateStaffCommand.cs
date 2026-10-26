using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.DeactivateStaff;

/// <summary>Intent to remove a staff member's access to the acting actor's own centre. No centre: scope comes from the actor.</summary>
public sealed record DeactivateStaffCommand(Guid MembershipId, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffManage;
}
