using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Staff endpoints (Day 30 contract: docs/architecture/api-conventions.md). Each endpoint: bind → dispatch →
/// map the result. The centre never comes from the route or the body — only from the actor, through the
/// dispatcher's tenant step. Protected by the authorization fallback policy; none is anonymous.
/// </summary>
public static class StaffEndpoints
{
    public static RouteGroupBuilder MapStaffEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/staff", ListStaffAsync)
            .WithName("ListStaff")
            .Produces<StaffListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapPost("/staff", CreateStaffAsync)
            .WithName("CreateStaff")
            .Produces<CreateStaffResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        api.MapPut("/staff/{membershipId:guid}/role", ChangeStaffRoleAsync)
            .WithName("ChangeStaffRole")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/staff/{membershipId:guid}/deactivate", DeactivateStaffAsync)
            .WithName("DeactivateStaff")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/staff/{membershipId:guid}/reactivate", ReactivateStaffAsync)
            .WithName("ReactivateStaff")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    private static async Task<IResult> ListStaffAsync(Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(new ListStaffQuery(), ct);
        return result.ToHttpResult(items => Results.Ok(new StaffListResponse(items)));
    }

    private static async Task<IResult> CreateStaffAsync(CreateStaffRequest request, HttpContext httpContext, Dispatcher dispatcher, CancellationToken ct)
    {
        // Set unconditionally, before dispatch: a one-time secret (or the knowledge that one was issued) must
        // never end up in any cache, success or failure.
        httpContext.Response.Headers.CacheControl = "no-store";

        var result = await dispatcher.SendAsync<CreateStaffCommand, CreateStaffResult>(
            new CreateStaffCommand(request.Email, request.DisplayName, request.Role, request.PreferredLocale), ct);
        return result.ToCreatedHttpResult(value =>
            (object)new CreateStaffResponse(value.MembershipId, value.UserId, value.TemporaryPassword));
    }

    private static async Task<IResult> ChangeStaffRoleAsync(Guid membershipId, ChangeStaffRoleRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ChangeStaffRoleCommand, Unit>(
            new ChangeStaffRoleCommand(membershipId, request.Role, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> DeactivateStaffAsync(Guid membershipId, StaffVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<DeactivateStaffCommand, Unit>(new DeactivateStaffCommand(membershipId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ReactivateStaffAsync(Guid membershipId, StaffVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ReactivateStaffCommand, Unit>(new ReactivateStaffCommand(membershipId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Collections are wrapped in an object (never a bare array), matching the subjects list shape.</summary>
public sealed record StaffListResponse(IReadOnlyList<StaffMemberDto> Items);

/// <summary>Exactly the contract's four fields: a client cannot set centreId, status or userId by over-posting (strict JSON rejects unknown members).</summary>
public sealed record CreateStaffRequest(string Email, string DisplayName, StaffRole Role, string PreferredLocale);

/// <summary>temporaryPassword is null when the email already had an account — the caller must tell the person to sign in with their existing password.</summary>
public sealed record CreateStaffResponse(Guid MembershipId, Guid UserId, string? TemporaryPassword);

/// <summary>Version is `required`: a positional-record `uint` silently defaults to 0 when absent from the JSON, masquerading as a stale version instead of a 400.</summary>
public sealed record ChangeStaffRoleRequest
{
    public required StaffRole Role { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Shared by deactivate and reactivate: both are version-guarded state transitions with no other input.</summary>
public sealed record StaffVersionRequest
{
    public required uint Version { get; init; }
}
