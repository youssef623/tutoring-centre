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
        var validationError = RequestValidation.ToValidationError(await new LoginRequestValidator().ValidateAsync(request, ct));
        if (validationError is not null)
        {
            return validationError.ToProblemResult();
        }

        var authResult = await authenticationService.VerifyCredentialsAsync(request.Email, request.Password, ct);
        if (authResult.IsFailure)
        {
            return authResult.Error!.ToProblemResult();
        }

        // The request is now authenticated as this user, with no centre yet. A request that already carried a
        // valid session arrives with ActorMiddleware having set the actor from the old cookie; the freshly
        // verified identity must replace it, not collide with it.
        var authenticatedUser = authResult.Value;
        currentActor.Reauthenticate(new StaffActor(authenticatedUser.UserId, null, null));

        var meResult = await dispatcher.QueryAsync<GetMyMembershipsQuery, MeDto>(new GetMyMembershipsQuery(), ct);
        if (meResult.IsFailure)
        {
            return meResult.Error!.ToProblemResult();
        }

        var me = meResult.Value;
        // Day 30 first-login gate: a pending password change blocks auto-selection even for a single-centre
        // user — the session carries no centre until the password is replaced.
        var autoSelected = !me.MustChangePassword && me.Memberships.Count == 1 ? me.Memberships[0] : null;

        // The cookie is never issued before this membership read.
        var principal = SessionPrincipalFactory.Create(
            authenticatedUser.UserId,
            authenticatedUser.SecurityStamp,
            autoSelected?.CentreId,
            autoSelected?.Role);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // me.Permissions was computed before the centre above was auto-selected (the actor had no centre yet),
        // so it is stale here the same way ActiveCentreId/ActiveRole are — recompute it for the selected role.
        var permissions = autoSelected is null
            ? Array.Empty<string>()
            : RolePermissions.For(autoSelected.Role).Order(StringComparer.Ordinal).ToArray();

        var response = me with { ActiveCentreId = autoSelected?.CentreId, ActiveRole = autoSelected?.Role, Permissions = permissions };
        return Results.Ok(response);
    }

    private static async Task<IResult> LogoutAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.NoContent();
    }

    private static async Task<IResult> GetMeAsync(Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetMyMembershipsQuery, MeDto>(new GetMyMembershipsQuery(), ct);
        return result.ToHttpResult(me => Results.Ok(me));
    }

    private static async Task<IResult> SelectCentreAsync(
        SelectCentreRequest request,
        HttpContext httpContext,
        ICurrentActor currentActor,
        Dispatcher dispatcher,
        CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetActiveMembershipQuery, ActiveMembershipDto>(
            new GetActiveMembershipQuery(request.CentreId), ct);

        if (result.IsFailure)
        {
            // The existing cookie is unchanged: nothing is written into the session before the membership check succeeds.
            return result.Error!.ToProblemResult();
        }

        // User ID and security stamp are carried over from the current session, never taken from the request.
        var staffActor = (StaffActor)currentActor.Actor;
        var stamp = httpContext.User.FindFirstValue(SessionClaimNames.SecurityStamp) ?? string.Empty;

        var principal = SessionPrincipalFactory.Create(staffActor.UserId, stamp, result.Value.CentreId, result.Value.Role);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Results.NoContent();
    }

    private static async Task<IResult> ChangePasswordAsync(
        ChangePasswordRequest request,
        HttpContext httpContext,
        AuthService authenticationService,
        ICurrentActor currentActor,
        CancellationToken ct)
    {
        var validationError = RequestValidation.ToValidationError(await new ChangePasswordRequestValidator().ValidateAsync(request, ct));
        if (validationError is not null)
        {
            return validationError.ToProblemResult();
        }

        var staffActor = (StaffActor)currentActor.Actor;
        var result = await authenticationService.ChangePasswordAsync(staffActor.UserId, request.CurrentPassword, request.NewPassword, ct);
        if (result.IsFailure)
        {
            return result.Error!.ToProblemResult();
        }

        // No centre, same as a fresh login: other sessions of this user fail their next revalidation on the
        // stamp mismatch; this session must select a centre again too, even if one was selected before.
        var principal = SessionPrincipalFactory.Create(result.Value.UserId, result.Value.SecurityStamp, null, null);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Results.NoContent();
    }
}
