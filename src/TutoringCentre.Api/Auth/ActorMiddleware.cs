using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Application.Identity.Portal;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// The single translation point from HTTP identity (session cookie claims) to Application identity
/// (<see cref="Actor"/>). No handler and no other endpoint reads claims. Runs after authentication,
/// before authorization. Unauthenticated requests are untouched; a request whose claims cannot be parsed
/// stays anonymous (authorization then rejects it) rather than trusting a partially-read session.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the ASP.NET Core middleware pipeline.")]
internal sealed partial class ActorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CurrentActorContext currentActor, ILogger<ActorMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(context);

        var subject = context.Items[SessionRevalidationHandler.PortalSubjectItemKey] as PortalSubject;
        if (context.User.Identity?.IsAuthenticated == true && TryBuildActor(context.User, subject, logger, out var userActor))
        {
            currentActor.Set(userActor);
        }

        await next(context);
    }

    private static bool TryBuildActor(ClaimsPrincipal principal, PortalSubject? subject, ILogger logger, [NotNullWhen(true)] out Actor? actor)
    {
        actor = null;

        var userIdClaim = principal.FindFirstValue(SessionClaimNames.UserId);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            LogMissingUserId(logger);
            return false;
        }

        var centreClaim = principal.FindFirstValue(SessionClaimNames.CentreId);
        var roleClaim = principal.FindFirstValue(SessionClaimNames.Role);

        if (centreClaim is null && roleClaim is null)
        {
            actor = new StaffActor(userId, null, null);
            return true;
        }

        if (centreClaim is not null
            && roleClaim is not null
            && Guid.TryParse(centreClaim, out var centreId)
            && Enum.TryParse<StaffRole>(roleClaim, out var role))
        {
            if (RoleScopes.IsStaffRole(role))
                actor = new StaffActor(userId, centreId, role);
            else if (subject is { Id: var id } && id != Guid.Empty)
            {
                if (role == StaffRole.Parent && subject.Kind == PortalSubjectKind.Parent)
                    actor = new ParentActor(userId, centreId, id);
                else if (role == StaffRole.Student && subject.Kind == PortalSubjectKind.Student)
                    actor = new StudentActor(userId, centreId, id);
            }
            if (actor is not null)
                return true;
        }

        LogInvalidCentreOrRole(logger, userId);
        return false;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Malformed session: missing or invalid user id claim")]
    private static partial void LogMissingUserId(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Malformed session for user {UserId}: invalid centre or role claim")]
    private static partial void LogInvalidCentreOrRole(ILogger logger, Guid userId);
}
