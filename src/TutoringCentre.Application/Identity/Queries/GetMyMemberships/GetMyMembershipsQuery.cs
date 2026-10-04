using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Identity.Queries.GetMyMemberships;

/// <summary>Asks for the signed-in staff member's own profile and active memberships. Takes identity from the actor.</summary>
public sealed record GetMyMembershipsQuery : IQuery<MeDto>;
