using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Claim names for the session cookie, defined once and read nowhere else but here and <c>ActorMiddleware</c>.
/// Minimal by design: no email, no display name (those come from GET /api/me), no permissions (Month 2
/// resolves them server-side) — fewer claims means less to leak and less to go stale.
/// </summary>
internal static class SessionClaimNames
{
    public const string UserId = "sub";
    public const string SecurityStamp = "stamp";
    public const string CentreId = "centre";
    public const string Role = "role";
}

/// <summary>Builds the principal the cookie scheme signs in — the server-issued, encrypted claim set.</summary>
internal static class SessionPrincipalFactory
{
    public static ClaimsPrincipal Create(Guid userId, string securityStamp, Guid? centreId, StaffRole? role)
    {
        var claims = new List<Claim>
        {
            new(SessionClaimNames.UserId, userId.ToString()),
            new(SessionClaimNames.SecurityStamp, securityStamp),
        };

        // Centre and role travel together: a role with no selected centre is meaningless.
        if (centreId is not null && role is not null)
        {
            claims.Add(new Claim(SessionClaimNames.CentreId, centreId.Value.ToString()));
            claims.Add(new Claim(SessionClaimNames.Role, role.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
