using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Portal;

public sealed record PortalAccountDto(Guid LinkId, Guid MembershipId, string Email, MembershipStatus Status,
    uint Version, DateTimeOffset CreatedAt);
