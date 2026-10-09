using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.ChangeStaffRole;

/// <summary>Intent to change a staff member's role within the acting actor's own centre. No centre: scope comes from the actor.</summary>
public sealed record ChangeStaffRoleCommand(Guid MembershipId, StaffRole Role, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffManage;
}
