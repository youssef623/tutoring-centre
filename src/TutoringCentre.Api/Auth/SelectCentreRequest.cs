namespace TutoringCentre.Api.Auth;

/// <summary>
/// The centre ID here is only a request — nothing is written into the session before
/// GetActiveMembershipQuery confirms the current user actually belongs to it.
/// </summary>
public sealed record SelectCentreRequest(Guid CentreId);
