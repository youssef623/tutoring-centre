# `src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Enrollment endpoints (Day 49 contract). Each endpoint: bind → dispatch → map the result. No rule, repository, entity, tenant, role or scope logic: the handlers resolve every ID inside the actor's centre and apply the actor's teacher scope to the two reads, so one route serves owners, secretaries and teachers alike. Enrolling is a nested resource of the group: the group comes from the route only, never from the body. Withdraw and transfer are version-guarded actions on the enrollment (Month 2's convention), never an HTTP DELETE: an ended enrollment stays as history. Transfer is one endpoint dispatching one command, so ending the old enrollment and starting the new one commit together in the dispatcher's one transaction. The student's enrollment list sits under /api/students because that is where a client looks, but it is an enrollment read with enrollment's own permission and contract; GET /api/students/{id} never embeds it. Protected by the authorization fallback policy; none is anonymous.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 140 lines and 7,737 bytes. Source hash: `8a2818c3add35eb1570f5e5c523ba960bdc0733c8eb4587bd7a3a3b067c57a0b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 4: `using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;`
- Line 5: `using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;`
- Line 6: `using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;`
- Line 7: `using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;`
- Line 8: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L24): `public static class EnrollmentEndpoints`
- [Line 26](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L26): `public static RouteGroupBuilder MapEnrollmentEndpoints(this RouteGroupBuilder api)`
- [Line 77](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L77): `private static async Task<IResult> EnrollStudentAsync(Guid groupId, EnrollStudentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 83](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L83): `private static async Task<IResult> GetGroupRosterAsync(Guid groupId, Dispatcher dispatcher, CancellationToken ct)`
- [Line 89](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L89): `private static async Task<IResult> ListStudentEnrollmentsAsync(Guid studentId, Dispatcher dispatcher, CancellationToken ct)`
- [Line 96](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L96): `private static async Task<IResult> WithdrawEnrollmentAsync(Guid id, EnrollmentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 103](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L103): `private static async Task<IResult> TransferEnrollmentAsync(Guid id, TransferEnrollmentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 115](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L115): `public sealed record EnrollStudentRequest`
- [Line 120](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L120): `public sealed record EnrollStudentResponse(Guid Id);`
- [Line 123](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L123): `public sealed record EnrollmentVersionRequest`
- [Line 129](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L129): `public sealed record TransferEnrollmentRequest`
- [Line 137](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L137): `public sealed record TransferEnrollmentResponse(Guid Id);`
- [Line 140](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L140): `public sealed record StudentEnrollmentListResponse(IReadOnlyList<StudentEnrollmentDto> Items);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Enrollment endpoints (Day 49 contract). Each endpoint: bind → dispatch → map the result. No rule, repository,
/// entity, tenant, role or scope logic: the handlers resolve every ID inside the actor's centre and apply the
/// actor's teacher scope to the two reads, so one route serves owners, secretaries and teachers alike.
/// Enrolling is a nested resource of the group: the group comes from the route only, never from the body.
/// Withdraw and transfer are version-guarded actions on the enrollment (Month 2's convention), never an HTTP
/// DELETE: an ended enrollment stays as history. Transfer is one endpoint dispatching one command, so ending the
/// old enrollment and starting the new one commit together in the dispatcher's one transaction.
/// The student's enrollment list sits under /api/students because that is where a client looks, but it is an
/// enrollment read with enrollment's own permission and contract; GET /api/students/{id} never embeds it.
/// Protected by the authorization fallback policy; none is anonymous.
/// </summary>
public static class EnrollmentEndpoints
{
    public static RouteGroupBuilder MapEnrollmentEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapPost("/groups/{groupId:guid}/enrollments", EnrollStudentAsync)
            .WithName("enrollStudent")
            .Produces<EnrollStudentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapGet("/groups/{groupId:guid}/roster", GetGroupRosterAsync)
            .WithName("getGroupRoster")
            .Produces<GroupRosterDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapGet("/students/{studentId:guid}/enrollments", ListStudentEnrollmentsAsync)
            .WithName("listStudentEnrollments")
            .Produces<StudentEnrollmentListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPost("/enrollments/{id:guid}/withdraw", WithdrawEnrollmentAsync)
            .WithName("withdrawEnrollment")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/enrollments/{id:guid}/transfer", TransferEnrollmentAsync)
            .WithName("transferEnrollment")
            .Produces<TransferEnrollmentResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    // The new enrollment has no page of its own, so Location points at the group's roster, where it is listed.
    // groupId is a route value, not a server-generated one, but it only reaches the header on success, after the
    // handler resolved that exact group inside the actor's centre; the :guid constraint keeps it a bare GUID.
    private static async Task<IResult> EnrollStudentAsync(Guid groupId, EnrollStudentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<EnrollStudentCommand, EnrollStudentResult>(new EnrollStudentCommand(groupId, request.StudentId), ct);
        return result.ToCreatedHttpResult(value => ($"/api/groups/{groupId}/roster", new EnrollStudentResponse(value.EnrollmentId)));
    }

    private static async Task<IResult> GetGroupRosterAsync(Guid groupId, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetGroupRosterQuery, GroupRosterDto>(new GetGroupRosterQuery(groupId), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> ListStudentEnrollmentsAsync(Guid studentId, Dispatcher dispatcher, CancellationToken ct)
    {
```

The excerpt stops at line 90; use the original file for the remaining 50 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
