namespace TutoringCentre.Domain.Identity;

/// <summary>
/// Which roles are limited to their own records on an ownership-guarded request, rather than the whole
/// centre's. Kept beside <see cref="RolePermissions"/>, in the Domain, so handlers never compare against
/// <see cref="StaffRole"/> directly to make this decision themselves.
/// </summary>
public static class RoleScopes
{
    public static bool IsStaffRole(StaffRole role) => role is StaffRole.Owner or StaffRole.Secretary or StaffRole.Teacher;

    /// <summary>True only for Teacher: Owner and Secretary always see every group in the centre.</summary>
    public static bool IsLimitedToOwnGroups(StaffRole role) => role == StaffRole.Teacher;
}
