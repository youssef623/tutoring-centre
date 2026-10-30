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
    internal static readonly object PortalSubjectItemKey = new();

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

        var centreClaim = context.Principal?.FindFirstValue(SessionClaimNames.CentreId);
        var roleClaim = context.Principal?.FindFirstValue(SessionClaimNames.Role);
        var centreId = Guid.TryParse(centreClaim, out var parsedCentreId)
            ? parsedCentreId
            : (Guid?)null;
        var role = Enum.TryParse<StaffRole>(roleClaim, out var parsedRole) && Enum.IsDefined(parsedRole)
            ? parsedRole
            : (StaffRole?)null;

        if ((centreClaim is not null || roleClaim is not null) && (centreId is null || role is null))
        {
            await RejectAsync(context);
            return;
        }

        var services = context.HttpContext.RequestServices;
        var cache = services.GetRequiredService<IMemoryCache>();
        var options = services.GetRequiredService<IOptions<SessionValidationOptions>>().Value;
        var cacheKey = ($"session-valid", userId, stamp, centreId, role);

        if (!cache.TryGetValue(cacheKey, out SessionValidationResult validation))
        {
            var dispatcher = services.GetRequiredService<Dispatcher>();
            var result = await dispatcher.QueryAsync<ValidateStaffSessionQuery, SessionValidationResult>(
                new ValidateStaffSessionQuery(userId, stamp, centreId, role), context.HttpContext.RequestAborted);
            validation = result.IsSuccess ? result.Value : new(false);

            if (validation.IsValid && options.CacheDuration > TimeSpan.Zero)
            {
                cache.Set(cacheKey, validation, options.CacheDuration);
            }
        }

        if (!validation.IsValid)
        {
            await RejectAsync(context);
        }
        else if (validation.PortalSubject is { } subject)
        {
            // Server-only request data. Never add the subject or children to cookie claims.
            context.HttpContext.Items[PortalSubjectItemKey] = subject;
        }
    }

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        // The cookie is deleted too: a rejected session should not keep presenting the same stale cookie.
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
