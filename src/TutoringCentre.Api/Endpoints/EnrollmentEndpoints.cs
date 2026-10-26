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
        var result = await dispatcher.QueryAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            new ListStudentEnrollmentsQuery(studentId), ct);
        return result.ToHttpResult(items => Results.Ok(new StudentEnrollmentListResponse(items)));
    }

    private static async Task<IResult> WithdrawEnrollmentAsync(Guid id, EnrollmentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<WithdrawEnrollmentCommand, Unit>(new WithdrawEnrollmentCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    // 200, not 201 or 204: one request ends one resource and creates another, and the body names the new one.
    private static async Task<IResult> TransferEnrollmentAsync(Guid id, TransferEnrollmentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            new TransferEnrollmentCommand(id, request.TargetGroupId, request.Version), ct);
        return result.ToHttpResult(value => Results.Ok(new TransferEnrollmentResponse(value.EnrollmentId)));
    }
}

/// <summary>
/// Student ID only. The group comes from the route and the centre from the actor; a groupId here is an unknown
/// field and a 400. `required`: an absent Guid would otherwise silently default to an empty ID.
/// </summary>
public sealed record EnrollStudentRequest
{
    public required Guid StudentId { get; init; }
}

public sealed record EnrollStudentResponse(Guid Id);

/// <summary>Version only: withdraw is a version-guarded state transition with no other input.</summary>
public sealed record EnrollmentVersionRequest
{
    public required uint Version { get; init; }
}

/// <summary>Target group plus version only. The target is resolved inside the actor's centre by the handler.</summary>
public sealed record TransferEnrollmentRequest
{
    public required Guid TargetGroupId { get; init; }

    public required uint Version { get; init; }
}

/// <summary>The new enrollment in the target group; the old one has ended with reason transferred.</summary>
public sealed record TransferEnrollmentResponse(Guid Id);

/// <summary>Collections are wrapped in an object (never a bare array) so the shape can grow without a breaking change.</summary>
public sealed record StudentEnrollmentListResponse(IReadOnlyList<StudentEnrollmentDto> Items);
