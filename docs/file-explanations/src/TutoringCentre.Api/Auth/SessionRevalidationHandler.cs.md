# `src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`

## Purpose

The cookie validation hook reads user, stamp, selected centre, and role before an Application actor exists. It caches positive query results by that tuple, rejects invalid principals, and clears invalid cookies. Role freshness now participates alongside stamp and membership activity. The positive cache creates a bounded delay rather than instant revocation.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 70 lines and 3,104 bytes. Source hash: `1cecaf6b7733a3923177b92b45d92e47e8ba83e096ee70ca4d87a0bfc04fe6ce`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Security.Claims;`
- Line 2: `using Microsoft.AspNetCore.Authentication;`
- Line 3: `using Microsoft.AspNetCore.Authentication.Cookies;`
- Line 4: `using Microsoft.Extensions.Caching.Memory;`
- Line 5: `using Microsoft.Extensions.DependencyInjection;`
- Line 6: `using Microsoft.Extensions.Options;`
- Line 7: `using TutoringCentre.Application.Common.Cqrs;`
- Line 8: `using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;`
- Line 9: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L19): `internal static class SessionRevalidationHandler`
- [Line 21](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L21): `public static async Task ValidateAsync(CookieValidatePrincipalContext context)`
- [Line 64](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L64): `private static async Task RejectAsync(CookieValidatePrincipalContext context)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthenticationSetup.cs:34](../../../../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs#L34)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Re-checks a validated cookie's claims against the database: a revoked membership or a changed security
/// stamp ends the session on the next request, not only on the next login. A valid result is cached briefly
/// per (user, stamp, centre); an invalid one is never cached, so revocation is never masked by a stale hit.
/// Runs before an actor exists — <see cref="ValidateStaffSessionQuery"/> takes the user ID explicitly.
/// </summary>
internal static class SessionRevalidationHandler
{
    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var userIdClaim = context.Principal?.FindFirstValue(SessionClaimNames.UserId);
        var stamp = context.Principal?.FindFirstValue(SessionClaimNames.SecurityStamp);
        if (userIdClaim is null || stamp is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            await RejectAsync(context);
            return;
        }

        var centreId = Guid.TryParse(context.Principal?.FindFirstValue(SessionClaimNames.CentreId), out var parsedCentreId)
            ? parsedCentreId
            : (Guid?)null;
        var role = Enum.TryParse<StaffRole>(context.Principal?.FindFirstValue(SessionClaimNames.Role), out var parsedRole)
            ? parsedRole
            : (StaffRole?)null;

        var services = context.HttpContext.RequestServices;
        var cache = services.GetRequiredService<IMemoryCache>();
        var options = services.GetRequiredService<IOptions<SessionValidationOptions>>().Value;
        var cacheKey = ($"session-valid", userId, stamp, centreId, role);

        if (!cache.TryGetValue(cacheKey, out bool isValid))
        {
            var dispatcher = services.GetRequiredService<Dispatcher>();
            var result = await dispatcher.QueryAsync<ValidateStaffSessionQuery, bool>(
                new ValidateStaffSessionQuery(userId, stamp, centreId, role), context.HttpContext.RequestAborted);
            isValid = result.IsSuccess && result.Value;

            if (isValid && options.CacheDuration > TimeSpan.Zero)
            {
                cache.Set(cacheKey, true, options.CacheDuration);
            }
        }

        if (!isValid)
        {
            await RejectAsync(context);
        }
    }

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        // The cookie is deleted too: a rejected session should not keep presenting the same stale cookie.
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
