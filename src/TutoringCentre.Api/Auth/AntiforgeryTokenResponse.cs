namespace TutoringCentre.Api.Auth;

/// <summary>The request token only — the cookie half of the pair is set directly on the response, never in this body.</summary>
public sealed record AntiforgeryTokenResponse(string Token);
