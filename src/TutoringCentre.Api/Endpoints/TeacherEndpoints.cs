using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Teacher endpoints (Day 43 contract). Each endpoint: bind → dispatch → map the result. No rule, repository,
/// entity or tenant logic — the tenant comes only from the actor, through the dispatcher's own layer 1 step.
/// The account link is its own sub-resource (PUT .../account): a different decision from editing a name, with
/// a different audit meaning, so it gets its own request model and `userId` is never accepted anywhere else.
/// Protected by the authorization fallback policy; none is anonymous.
/// </summary>
public static class TeacherEndpoints
{
    public static RouteGroupBuilder MapTeacherEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/teachers", ListTeachersAsync)
            .WithName("ListTeachers")
            .Produces<TeacherListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapGet("/teachers/{id:guid}", GetTeacherAsync)
            .WithName("GetTeacher")
            .Produces<TeacherDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPost("/teachers", CreateTeacherAsync)
            .WithName("CreateTeacher")
            .Produces<CreateTeacherResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapPut("/teachers/{id:guid}", UpdateTeacherAsync)
            .WithName("UpdateTeacher")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPut("/teachers/{id:guid}/account", SetTeacherAccountAsync)
            .WithName("SetTeacherAccount")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/teachers/{id:guid}/archive", ArchiveTeacherAsync)
            .WithName("ArchiveTeacher")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/teachers/{id:guid}/restore", RestoreTeacherAsync)
            .WithName("RestoreTeacher")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    // Defaults to false so the query string parameter is optional.
    private static async Task<IResult> ListTeachersAsync(Dispatcher dispatcher, CancellationToken ct, bool includeArchived = false)
    {
        var result = await dispatcher.QueryAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(new ListTeachersQuery(includeArchived), ct);
        return result.ToHttpResult(items => Results.Ok(new TeacherListResponse(items)));
    }

    private static async Task<IResult> GetTeacherAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetTeacherQuery, TeacherDto>(new GetTeacherQuery(id), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> CreateTeacherAsync(CreateTeacherRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CreateTeacherCommand, CreateTeacherResult>(
            new CreateTeacherCommand(request.FullName, request.Phone), ct);
        return result.ToCreatedHttpResult(value => ($"/api/teachers/{value.TeacherId}", new CreateTeacherResponse(value.TeacherId)));
    }

    private static async Task<IResult> UpdateTeacherAsync(Guid id, UpdateTeacherRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UpdateTeacherCommand, Unit>(
            new UpdateTeacherCommand(id, request.FullName, request.Phone, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> SetTeacherAccountAsync(Guid id, SetTeacherAccountRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<SetTeacherAccountCommand, Unit>(
            new SetTeacherAccountCommand(id, request.UserId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ArchiveTeacherAsync(Guid id, TeacherVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ArchiveTeacherCommand, Unit>(new ArchiveTeacherCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> RestoreTeacherAsync(Guid id, TeacherVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RestoreTeacherCommand, Unit>(new RestoreTeacherCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Collections are wrapped in an object (never a bare array) so the shape can grow without a breaking change.</summary>
public sealed record TeacherListResponse(IReadOnlyList<TeacherDto> Items);

/// <summary>Exactly the two fields CreateTeacherCommand accepts: no userId here, linking is its own sub-resource.</summary>
public sealed record CreateTeacherRequest(string FullName, string? Phone);

public sealed record CreateTeacherResponse(Guid Id);

/// <summary>
/// The ID comes only from the route; this body carries no ID, and no userId — editing details never touches
/// the account link. Version is `required`: a positional-record `uint` silently defaults to 0 when the
/// property is absent from the JSON, which would masquerade as a (very) stale version instead of the 400 a
/// missing field should produce.
/// </summary>
public sealed record UpdateTeacherRequest
{
    public required string FullName { get; init; }

    public string? Phone { get; init; }

    public required uint Version { get; init; }
}

/// <summary>The account sub-resource's own request: userId is accepted here, and nowhere else. Null clears the link.</summary>
public sealed record SetTeacherAccountRequest
{
    public Guid? UserId { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Shared by archive and restore: both are version-guarded state transitions with no other input.</summary>
public sealed record TeacherVersionRequest
{
    public required uint Version { get; init; }
}
