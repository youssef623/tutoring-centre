# `src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Staff endpoints (Day 30 contract: docs/architecture/api-conventions.md). Each endpoint: bind → dispatch → map the result. The centre never comes from the route or the body — only from the actor, through the dispatcher's tenant step. Protected by the authorization fallback policy; none is anonymous.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 121 lines and 6,278 bytes. Source hash: `852118fe33aadc2c2dafeddeaf6203b1cd944a20c478427e69d16ff35544e247`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Staff;`
- Line 4: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 5: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 6: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 7: `using TutoringCentre.Application.Staff.Commands.ReactivateStaff;`
- Line 8: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 9: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L18): `public static class StaffEndpoints`
- [Line 20](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L20): `public static RouteGroupBuilder MapStaffEndpoints(this RouteGroupBuilder api)`
- [Line 62](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L62): `private static async Task<IResult> ListStaffAsync(Dispatcher dispatcher, CancellationToken ct)`
- [Line 68](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L68): `private static async Task<IResult> CreateStaffAsync(CreateStaffRequest request, HttpContext httpContext, Dispatcher dispatcher, CancellationToken ct)`
- [Line 80](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L80): `private static async Task<IResult> ChangeStaffRoleAsync(Guid membershipId, ChangeStaffRoleRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 87](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L87): `private static async Task<IResult> DeactivateStaffAsync(Guid membershipId, StaffVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 93](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L93): `private static async Task<IResult> ReactivateStaffAsync(Guid membershipId, StaffVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 101](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L101): `public sealed record StaffListResponse(IReadOnlyList<StaffMemberDto> Items);`
- [Line 104](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L104): `public sealed record CreateStaffRequest(string Email, string DisplayName, StaffRole Role, string PreferredLocale);`
- [Line 107](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L107): `public sealed record CreateStaffResponse(Guid MembershipId, Guid UserId, string? TemporaryPassword);`
- [Line 110](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L110): `public sealed record ChangeStaffRoleRequest`
- [Line 118](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L118): `public sealed record StaffVersionRequest`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 31 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
