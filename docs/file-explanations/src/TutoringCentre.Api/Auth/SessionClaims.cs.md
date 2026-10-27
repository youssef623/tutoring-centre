# `src/TutoringCentre.Api/Auth/SessionClaims.cs`

## Purpose

Source summary (intent, not independent execution proof): Claim names for the session cookie, defined once and read nowhere else but here and ActorMiddleware. Minimal by design: no email, no display name (those come from GET /api/me), no permissions (Month 2 resolves them server-side) — fewer claims means less to leak and less to go stale.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/SessionClaims.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,666 bytes. Source hash: `77a8b90a4337798f0a56f067d9620a3ed61b4b355224df7abe91aa06a805dc60`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Security.Claims;`
- Line 2: `using Microsoft.AspNetCore.Authentication.Cookies;`
- Line 3: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Api/Auth/SessionClaims.cs#L12): `internal static class SessionClaimNames`
- [Line 21](../../../../../src/TutoringCentre.Api/Auth/SessionClaims.cs#L21): `internal static class SessionPrincipalFactory`
- [Line 23](../../../../../src/TutoringCentre.Api/Auth/SessionClaims.cs#L23): `public static ClaimsPrincipal Create(Guid userId, string securityStamp, Guid? centreId, StaffRole? role)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/ActorMiddleware.cs:33](../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L33)
- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:121](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L121)
- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:168](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L168)
- [src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs:61](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs#L61)
- [src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs:25](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L25)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
