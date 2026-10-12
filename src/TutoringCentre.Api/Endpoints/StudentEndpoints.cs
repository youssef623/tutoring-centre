using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;
using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;
using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;
using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Student endpoints (Day 41 contract: docs/architecture/api-conventions.md). Each endpoint: bind → dispatch →
/// map the result. No rule, repository, entity or tenant logic — the tenant comes only from the actor, through
/// the dispatcher's own layer 1 step. Protected by the authorization fallback policy; none is anonymous.
/// Unlink and make-primary are action sub-resources carrying the student's version in the body, following
/// Month 2's convention for state changes, instead of an HTTP DELETE with a body: the parent id comes from the
/// route and the version from the body, so the two cannot disagree about which student is being changed.
/// </summary>
public static class StudentEndpoints
{
    private const int DefaultPageSize = 25;

    public static RouteGroupBuilder MapStudentEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/students", ListStudentsAsync)
            .WithName("listStudents")
            .Produces<StudentPageDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapGet("/students/{id:guid}", GetStudentAsync)
            .WithName("getStudent")
            .Produces<StudentDetailDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPost("/students", RegisterStudentAsync)
            .WithName("registerStudent")
            .Produces<RegisterStudentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        api.MapPut("/students/{id:guid}", UpdateStudentAsync)
            .WithName("updateStudent")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/students/{id:guid}/archive", ArchiveStudentAsync)
            .WithName("archiveStudent")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/students/{id:guid}/restore", RestoreStudentAsync)
            .WithName("restoreStudent")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/students/{id:guid}/parents", LinkStudentParentAsync)
            .WithName("linkStudentParent")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/students/{id:guid}/parents/{parentId:guid}/unlink", UnlinkStudentParentAsync)
            .WithName("unlinkStudentParent")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/students/{id:guid}/parents/{parentId:guid}/make-primary", MakeStudentParentPrimaryAsync)
            .WithName("makeStudentParentPrimary")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    // Defaults mirror the Day 40 contract: archived excluded, page size 25. The search term never reaches
    // request logging as a distinct field — Serilog's request-logging line only ever records RequestPath,
    // never the query string, so a name typed into this box is never written to a log.
    private static async Task<IResult> ListStudentsAsync(
        Dispatcher dispatcher, CancellationToken ct, string? search = null, bool includeArchived = false, string? cursor = null, int pageSize = DefaultPageSize)
    {
        var result = await dispatcher.QueryAsync<ListStudentsQuery, StudentPageDto>(
            new ListStudentsQuery(search, includeArchived, cursor, pageSize), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> GetStudentAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetStudentQuery, StudentDetailDto>(new GetStudentQuery(id), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> RegisterStudentAsync(RegisterStudentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var parent = new RegisterStudentParent(request.Parent.Id, request.Parent.FullName, request.Parent.Phone);
        var result = await dispatcher.SendAsync<RegisterStudentCommand, RegisterStudentResult>(
            new RegisterStudentCommand(request.FullName, request.Phone, request.SchoolYear, parent, request.Relationship), ct);
        return result.ToCreatedHttpResult(value => ($"/api/students/{value.StudentId}", new RegisterStudentResponse(value.StudentId, value.ParentId)));
    }

    private static async Task<IResult> UpdateStudentAsync(Guid id, UpdateStudentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UpdateStudentCommand, Unit>(
            new UpdateStudentCommand(id, request.FullName, request.Phone, request.SchoolYear, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ArchiveStudentAsync(Guid id, StudentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ArchiveStudentCommand, Unit>(new ArchiveStudentCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> RestoreStudentAsync(Guid id, StudentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RestoreStudentCommand, Unit>(new RestoreStudentCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> LinkStudentParentAsync(Guid id, LinkStudentParentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<LinkParentCommand, Unit>(
            new LinkParentCommand(id, request.ParentId, request.Relationship, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> UnlinkStudentParentAsync(
        Guid id, Guid parentId, StudentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UnlinkParentCommand, Unit>(new UnlinkParentCommand(id, parentId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> MakeStudentParentPrimaryAsync(
        Guid id, Guid parentId, StudentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<SetPrimaryParentCommand, Unit>(new SetPrimaryParentCommand(id, parentId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Exactly one of <see cref="Id"/> alone, or <see cref="FullName"/> and <see cref="Phone"/> together — RegisterStudentValidator checks the shape.</summary>
public sealed record RegisterStudentRequestParent
{
    public Guid? Id { get; init; }

    public string? FullName { get; init; }

    public string? Phone { get; init; }
}

/// <summary>No id, no centreId: the id is server-generated and the centre comes only from the actor.</summary>
public sealed record RegisterStudentRequest
{
    public required string FullName { get; init; }

    public string? Phone { get; init; }

    public string? SchoolYear { get; init; }

    public required RegisterStudentRequestParent Parent { get; init; }

    public required ParentRelationship Relationship { get; init; }
}

public sealed record RegisterStudentResponse(Guid Id, Guid ParentId);

/// <summary>
/// The ID comes only from the route; this body carries no ID and no status. Version is `required`: a
/// positional-record `uint` silently defaults to 0 when the property is absent from the JSON, which would
/// masquerade as a (very) stale version instead of the 400 a missing field should produce.
/// </summary>
public sealed record UpdateStudentRequest
{
    public required string FullName { get; init; }

    public string? Phone { get; init; }

    public string? SchoolYear { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Shared by archive, restore, unlink and make-primary: each is a version-guarded state transition with no other input.</summary>
public sealed record StudentVersionRequest
{
    public required uint Version { get; init; }
}

/// <summary>The parent is a new member of the student's own collection, so it travels in the body, not the route.</summary>
public sealed record LinkStudentParentRequest
{
    public required Guid ParentId { get; init; }

    public required ParentRelationship Relationship { get; init; }

    public required uint Version { get; init; }
}
