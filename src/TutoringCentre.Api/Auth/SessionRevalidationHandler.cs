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
