using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.ValidateStaffSession;

/// <summary>
/// Asks whether an existing session is still valid. Runs inside the authentication pipeline before an actor
/// exists, so it takes the user ID explicitly rather than reading it from the actor. Internal to that pipeline:
/// no endpoint may expose it, because it would let a caller probe security stamps and membership state directly.
/// <see cref="Role"/> is the role the session's cookie claims for <see cref="CentreId"/> (Day 29); a mismatch
/// against the membership's current role is treated exactly like a deactivated membership.
/// </summary>
public sealed record ValidateStaffSessionQuery(Guid UserId, string SecurityStamp, Guid? CentreId, StaffRole? Role) : IQuery<bool>;
