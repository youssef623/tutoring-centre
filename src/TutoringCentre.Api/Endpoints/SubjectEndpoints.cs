using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Subject endpoints (Day 26 contract: docs/architecture/api-conventions.md). Each endpoint: bind → dispatch →
/// map the result. No rule, repository, entity or tenant logic — the tenant comes only from the actor, through
/// the dispatcher's own layer 1 step. Protected by the authorization fallback policy; none is anonymous.
/// </summary>
public static class SubjectEndpoints
{
    public static RouteGroupBuilder MapSubjectEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/subjects", ListSubjectsAsync)
            .WithName("ListSubjects")
            .Produces<SubjectListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapGet("/subjects/{id:guid}", GetSubjectAsync)
            .WithName("GetSubject")
            .Produces<SubjectDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPost("/subjects", CreateSubjectAsync)
            .WithName("CreateSubject")
            .Produces<CreateSubjectResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        api.MapPut("/subjects/{id:guid}", RenameSubjectAsync)
            .WithName("RenameSubject")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/subjects/{id:guid}/archive", ArchiveSubjectAsync)
            .WithName("ArchiveSubject")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/subjects/{id:guid}/restore", RestoreSubjectAsync)
            .WithName("RestoreSubject")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    // Defaults to false so the query string parameter is optional, per the Day 26 contract.
    private static async Task<IResult> ListSubjectsAsync(Dispatcher dispatcher, CancellationToken ct, bool includeArchived = false)
    {
        var result = await dispatcher.QueryAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(new ListSubjectsQuery(includeArchived), ct);
        return result.ToHttpResult(items => Results.Ok(new SubjectListResponse(items)));
    }

    private static async Task<IResult> GetSubjectAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetSubjectQuery, SubjectDto>(new GetSubjectQuery(id), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> CreateSubjectAsync(CreateSubjectRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(new CreateSubjectCommand(request.Name), ct);
        return result.ToCreatedHttpResult(value => ($"/api/subjects/{value.SubjectId}", new CreateSubjectResponse(value.SubjectId)));
    }

    private static async Task<IResult> RenameSubjectAsync(Guid id, RenameSubjectRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RenameSubjectCommand, Unit>(new RenameSubjectCommand(id, request.Name, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ArchiveSubjectAsync(Guid id, SubjectVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ArchiveSubjectCommand, Unit>(new ArchiveSubjectCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> RestoreSubjectAsync(Guid id, SubjectVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RestoreSubjectCommand, Unit>(new RestoreSubjectCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Collections are wrapped in an object (never a bare array) so the shape can grow — e.g. paging later — without a breaking change.</summary>
public sealed record SubjectListResponse(IReadOnlyList<SubjectDto> Items);

/// <summary>Exactly one property: a client cannot set a field CreateSubjectCommand never meant to accept.</summary>
public sealed record CreateSubjectRequest(string Name);

public sealed record CreateSubjectResponse(Guid Id);

/// <summary>
/// The ID comes only from the route; this body carries no ID. Version is `required`: a positional-record
/// `uint` silently defaults to 0 when the property is absent from the JSON, which would masquerade as a (very)
/// stale version instead of the 400 a missing field should produce.
/// </summary>
public sealed record RenameSubjectRequest
{
    public required string Name { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Shared by archive and restore: both are version-guarded state transitions with no other input.</summary>
public sealed record SubjectVersionRequest
{
    public required uint Version { get; init; }
}
