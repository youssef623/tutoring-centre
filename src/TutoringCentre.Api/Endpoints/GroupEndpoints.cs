using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Group endpoints (Day 45 contract). Each endpoint: bind → dispatch → map the result. No rule, repository,
/// entity, tenant, role or scope logic: one route serves owners, secretaries and teachers alike, and the query
/// handlers apply the actor's teacher scope below this layer, so there is no "my groups" route to bypass.
/// Name, capacity and teacher are separate sub-resources with their own request models; none of them accepts a
/// subject ID, so the subject fixed at creation cannot be changed through any route.
/// Protected by the authorization fallback policy; none is anonymous.
/// </summary>
public static class GroupEndpoints
{
    public static RouteGroupBuilder MapGroupEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/groups", ListGroupsAsync)
            .WithName("ListGroups")
            .Produces<GroupListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapGet("/groups/{id:guid}", GetGroupAsync)
            .WithName("GetGroup")
            .Produces<GroupDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPost("/groups", CreateGroupAsync)
            .WithName("CreateGroup")
            .Produces<CreateGroupResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPut("/groups/{id:guid}", RenameGroupAsync)
            .WithName("RenameGroup")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPut("/groups/{id:guid}/capacity", ChangeGroupCapacityAsync)
            .WithName("ChangeGroupCapacity")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPut("/groups/{id:guid}/teacher", AssignGroupTeacherAsync)
            .WithName("AssignGroupTeacher")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/groups/{id:guid}/archive", ArchiveGroupAsync)
            .WithName("ArchiveGroup")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/groups/{id:guid}/restore", RestoreGroupAsync)
            .WithName("RestoreGroup")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    // All three filters are optional query string parameters; includeArchived defaults to false.
    private static async Task<IResult> ListGroupsAsync(
        Dispatcher dispatcher, CancellationToken ct, Guid? subjectId = null, Guid? teacherId = null, bool includeArchived = false)
    {
        var result = await dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
            new ListGroupsQuery(subjectId, teacherId, includeArchived), ct);
        return result.ToHttpResult(items => Results.Ok(new GroupListResponse(items)));
    }

    private static async Task<IResult> GetGroupAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetGroupQuery, GroupDto>(new GetGroupQuery(id), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> CreateGroupAsync(CreateGroupRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CreateGroupCommand, CreateGroupResult>(
            new CreateGroupCommand(request.Name, request.SubjectId, request.TeacherId, request.Capacity), ct);
        return result.ToCreatedHttpResult(value => ($"/api/groups/{value.GroupId}", new CreateGroupResponse(value.GroupId)));
    }

    private static async Task<IResult> RenameGroupAsync(Guid id, RenameGroupRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RenameGroupCommand, Unit>(new RenameGroupCommand(id, request.Name, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ChangeGroupCapacityAsync(
        Guid id, ChangeGroupCapacityRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ChangeGroupCapacityCommand, Unit>(
            new ChangeGroupCapacityCommand(id, request.Capacity, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> AssignGroupTeacherAsync(
        Guid id, AssignGroupTeacherRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<AssignGroupTeacherCommand, Unit>(
            new AssignGroupTeacherCommand(id, request.TeacherId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ArchiveGroupAsync(Guid id, GroupVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ArchiveGroupCommand, Unit>(new ArchiveGroupCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> RestoreGroupAsync(Guid id, GroupVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RestoreGroupCommand, Unit>(new RestoreGroupCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Collections are wrapped in an object (never a bare array) so the shape can grow without a breaking change.</summary>
public sealed record GroupListResponse(IReadOnlyList<GroupDto> Items);

/// <summary>
/// Exactly the four fields CreateGroupCommand accepts: no status (every group starts active) and no enrolled
/// count (derived, never client-supplied). Subject and teacher IDs are resolved inside the actor's centre by the
/// handler. `required` on the value types: a positional-record Guid or int silently defaults when the property is
/// absent from the JSON, which would masquerade as an empty ID or capacity 0 instead of the 400 a missing field
/// should produce.
/// </summary>
public sealed record CreateGroupRequest
{
    public required string Name { get; init; }

    public required Guid SubjectId { get; init; }

    public required Guid TeacherId { get; init; }

    public required int Capacity { get; init; }
}

public sealed record CreateGroupResponse(Guid Id);

/// <summary>Name plus version only. The ID comes from the route; a subjectId here is an unknown field and a 400.</summary>
public sealed record RenameGroupRequest
{
    public required string Name { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Capacity plus version only. The 1–200 range is the entity's rule, reported as a 400 on `capacity`.</summary>
public sealed record ChangeGroupCapacityRequest
{
    public required int Capacity { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Teacher ID plus version only. The teacher is resolved inside the actor's centre by the handler.</summary>
public sealed record AssignGroupTeacherRequest
{
    public required Guid TeacherId { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Shared by archive and restore: both are version-guarded state transitions with no other input.</summary>
public sealed record GroupVersionRequest
{
    public required uint Version { get; init; }
}
