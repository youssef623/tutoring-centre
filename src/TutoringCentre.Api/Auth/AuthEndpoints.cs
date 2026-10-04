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

        // The request is now authenticated as this user, with no centre yet — nothing else may Set the actor this scope.
        var authenticatedUser = authResult.Value;
        currentActor.Set(new StaffActor(authenticatedUser.UserId, null, null));

        var meResult = await dispatcher.QueryAsync<GetMyMembershipsQuery, MeDto>(new GetMyMembershipsQuery(), ct);
        if (meResult.IsFailure)
        {
            return meResult.Error!.ToProblemResult();
        }

        var me = meResult.Value;
        var autoSelected = me.Memberships.Count == 1 ? me.Memberships[0] : null;

        // The cookie is never issued before this membership read.
        var principal = SessionPrincipalFactory.Create(
            authenticatedUser.UserId,
            authenticatedUser.SecurityStamp,
            autoSelected?.CentreId,
            autoSelected?.Role);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var response = me with { ActiveCentreId = autoSelected?.CentreId, ActiveRole = autoSelected?.Role };
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
}
