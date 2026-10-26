using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff;

/// <summary>The three outcomes shared by every staff command's decision order (Day 29 contract): scoped lookup, self-change, and the last-owner guard.</summary>
internal static class StaffErrors
{
    public static Error NotFound() => Error.NotFound("staff.not_found", "This staff member could not be found.");

    public static Error CannotChangeSelf() => Error.Rule("staff.cannot_change_self", "You cannot change your own membership this way.");

    public static Error LastOwner() => Error.Rule("staff.last_owner", "This centre must keep at least one active owner.");
}
