# `src/TutoringCentre.Api/Auth/ActorMiddleware.cs`

## Purpose

This is the HTTP claims-to-Application identity translation point. It builds a staff actor only from a parseable user ID and a valid centre/role pairing, leaves malformed sessions without a trusted actor, and invokes the next middleware. Handlers consume ICurrentActor rather than parsing claims independently.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 67 lines and 2,736 bytes. Source hash: `194113963787d1176fdf34bfd5a80d3c51a46dbc2cc64364a1dfb5c2b3fd6db4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Security.Claims;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L15): `internal sealed partial class ActorMiddleware(RequestDelegate next)`
- [Line 17](../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L17): `public async Task InvokeAsync(HttpContext context, CurrentActorContext currentActor, ILogger<ActorMiddleware> logger)`
- [Line 29](../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L29): `private static bool TryBuildActor(ClaimsPrincipal principal, ILogger logger, [NotNullWhen(true)] out StaffActor? actor)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:194](../../../../../src/TutoringCentre.Api/Program.cs#L194)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// The single translation point from HTTP identity (session cookie claims) to Application identity
/// (<see cref="StaffActor"/>). No handler and no other endpoint reads claims. Runs after authentication,
/// before authorization. Unauthenticated requests are untouched; a request whose claims cannot be parsed
/// stays anonymous (authorization then rejects it) rather than trusting a partially-read session.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the ASP.NET Core middleware pipeline.")]
internal sealed partial class ActorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CurrentActorContext currentActor, ILogger<ActorMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.User.Identity?.IsAuthenticated == true && TryBuildActor(context.User, logger, out var staffActor))
        {
            currentActor.Set(staffActor);
        }

        await next(context);
    }

    private static bool TryBuildActor(ClaimsPrincipal principal, ILogger logger, [NotNullWhen(true)] out StaffActor? actor)
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
            actor = new StaffActor(userId, centreId, role);
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
