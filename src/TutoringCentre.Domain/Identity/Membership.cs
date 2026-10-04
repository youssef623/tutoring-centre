using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Identity;

/// <summary>
/// A staff member's role in one centre. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.
/// Deliberately not <see cref="ITenantOwned"/>: memberships are read at login, before a tenant is selected, so they
/// cannot be filtered by the current tenant the way centre-owned data is.
/// </summary>
public sealed class Membership : Entity
{
    private Membership(Guid userId, Guid centreId, StaffRole role)
    {
        UserId = userId;
        CentreId = centreId;
        Role = role;
        Status = MembershipStatus.Active;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Membership()
    {
    }

    public Guid UserId { get; private set; }

    public Guid CentreId { get; private set; }

    public StaffRole Role { get; private set; }

    public MembershipStatus Status { get; private set; }

    public static Result<Membership> Create(Guid userId, Guid centreId, StaffRole role)
    {
        if (userId == Guid.Empty)
        {
            return Result<Membership>.Failure(Error.Validation("membership.user_required", "A user is required."));
        }

        if (centreId == Guid.Empty)
        {
            return Result<Membership>.Failure(Error.Validation("membership.centre_required", "A centre is required."));
        }

        return Result<Membership>.Success(new Membership(userId, centreId, role));
    }

    public Result Deactivate()
    {
        if (Status == MembershipStatus.Inactive)
        {
            return Result.Failure(Error.Rule("membership.already_inactive", "This membership is already inactive."));
        }

        Status = MembershipStatus.Inactive;
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == MembershipStatus.Active)
        {
            return Result.Failure(Error.Rule("membership.already_active", "This membership is already active."));
        }

        Status = MembershipStatus.Active;
        return Result.Success();
    }
}
