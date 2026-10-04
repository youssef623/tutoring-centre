using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Identity.Queries.GetActiveMembership;

/// <summary>Asks whether the signed-in staff member may act in the given centre — the tenant gate.</summary>
public sealed record GetActiveMembershipQuery(Guid CentreId) : IQuery<ActiveMembershipDto>;
