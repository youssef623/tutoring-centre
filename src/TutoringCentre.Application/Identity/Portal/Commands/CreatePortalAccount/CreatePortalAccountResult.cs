namespace TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;

public sealed record CreatePortalAccountResult(Guid LinkId, Guid MembershipId, Guid UserId, string? TemporaryPassword);
