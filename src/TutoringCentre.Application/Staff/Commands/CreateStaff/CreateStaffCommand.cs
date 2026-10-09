using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.CreateStaff;

/// <summary>Intent to add a staff member to the acting actor's own centre. The centre comes from the actor, never the caller.</summary>
public sealed record CreateStaffCommand(string Email, string DisplayName, StaffRole Role, string PreferredLocale)
    : ICommand<CreateStaffResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffManage;
}

public sealed record CreateStaffResult(Guid MembershipId, Guid UserId, string? TemporaryPassword);
