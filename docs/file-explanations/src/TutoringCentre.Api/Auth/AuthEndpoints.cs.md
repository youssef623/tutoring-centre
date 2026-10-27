# `src/TutoringCentre.Api/Auth/AuthEndpoints.cs`

## Purpose

Transport orchestration handles antiforgery token issue, credentials, ticket creation, logout, profile, centre selection, and password change through application ports. Login suppresses auto-selection when password replacement is pending; password change issues a new centreless ticket. Identity and membership rules live behind ports rather than direct infrastructure dependencies.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 203 lines and 9,370 bytes. Source hash: `cf6265db2622f1fe45df7062742f635e868d5a349c2e0b9c2f318d5774840814`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Security.Claims;`
- Line 2: `using Microsoft.AspNetCore.Antiforgery;`
- Line 3: `using Microsoft.AspNetCore.Authentication;`
- Line 4: `using Microsoft.AspNetCore.Authentication.Cookies;`
- Line 5: `using TutoringCentre.Api.Http;`
- Line 6: `using TutoringCentre.Application.Common.Cqrs;`
- Line 7: `using TutoringCentre.Application.Common.Security;`
- Line 8: `using TutoringCentre.Application.Identity;`
- Line 9: `using TutoringCentre.Application.Identity.Queries.GetActiveMembership;`
- Line 10: `using TutoringCentre.Application.Identity.Queries.GetMyMemberships;`
- Line 11: `using TutoringCentre.Domain.Identity;`
- Line 12: `using AuthService = TutoringCentre.Application.Common.Security.IAuthenticationService;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L21): `public static class AuthEndpoints`
- [Line 23](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L23): `public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder api)`
- [Line 73](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L73): `private static IResult GetAntiforgeryTokenAsync(HttpContext httpContext, IAntiforgery antiforgery)`
- [Line 83](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L83): `private static async Task<IResult> LoginAsync(`
- [Line 138](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L138): `private static async Task<IResult> LogoutAsync(HttpContext httpContext)`
- [Line 144](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L144): `private static async Task<IResult> GetMeAsync(Dispatcher dispatcher, CancellationToken ct)`
- [Line 150](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L150): `private static async Task<IResult> SelectCentreAsync(`
- [Line 176](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L176): `private static async Task<IResult> ChangePasswordAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/assemble.py:138](../../../../onboarding-work/assemble.py#L138)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.GetActiveMembership;
using TutoringCentre.Application.Identity.Queries.GetMyMemberships;
using TutoringCentre.Domain.Identity;
using AuthService = TutoringCentre.Application.Common.Security.IAuthenticationService;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Session endpoints. Each one orchestrates transport around Application calls — no password logic, no
/// membership rules, no Infrastructure or Identity types. Authentication ends here: everything after an
/// endpoint deals with a StaffActor (via ActorMiddleware), never with cookies or claims.
/// </summary>
public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/auth/antiforgery", GetAntiforgeryTokenAsync)
            .WithName("GetAntiforgeryToken")
            .Produces<AntiforgeryTokenResponse>()
            .AllowAnonymous();

        api.MapPost("/auth/login", LoginAsync)
            .WithName("Login")
            .Produces<MeDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting(LoginRateLimiting.PolicyName)
            .AllowAnonymous();

        // A bare (HttpContext) => ... lambda here is ambiguous between the Delegate-based MapPost overload (which
        // returns RouteHandlerBuilder, needed for .Produces below) and the low-level RequestDelegate one; the
        // explicit cast forces the former.
        // No .RequireAuthorization() here: the authorization fallback policy (AuthenticationSetup) already
        // requires an authenticated user by default (docs/architecture/api-conventions.md).
        api.MapPost("/auth/logout", (Delegate)(Func<HttpContext, Task<IResult>>)(httpContext => LogoutAsync(httpContext)))
            .WithName("Logout")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        api.MapGet("/me", GetMeAsync)
            .WithName("GetMe")
            .Produces<MeDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        api.MapPost("/session/centre", SelectCentreAsync)
            .WithName("SelectCentre")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapPost("/auth/change-password", ChangePasswordAsync)
            .WithName("ChangePassword")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting(ChangePasswordRateLimiting.PolicyName);

        return api;
    }

    private static IResult GetAntiforgeryTokenAsync(HttpContext httpContext, IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(httpContext);

        // Never cached: a stale token in a shared/browser cache would be reused across sessions.
        httpContext.Response.Headers.CacheControl = "no-store";

        return Results.Ok(new AntiforgeryTokenResponse(tokens.RequestToken!));
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        HttpContext httpContext,
        AuthService authenticationService,
        CurrentActorContext currentActor,
        Dispatcher dispatcher,
        CancellationToken ct)
    {
```

The excerpt stops at line 90; use the original file for the remaining 113 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
