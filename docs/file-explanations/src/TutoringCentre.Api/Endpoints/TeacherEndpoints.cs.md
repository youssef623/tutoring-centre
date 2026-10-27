# `src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Teacher endpoints (Day 43 contract). Each endpoint: bind → dispatch → map the result. No rule, repository, entity or tenant logic — the tenant comes only from the actor, through the dispatcher's own layer 1 step. The account link is its own sub-resource (PUT .../account): a different decision from editing a name, with a different audit meaning, so it gets its own request model and `userId` is never accepted anywhere else. Protected by the authorization fallback policy; none is anonymous.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 159 lines and 7,863 bytes. Source hash: `4e2748faee068aba1ff10d3f76712d5d510ea958a54948f61622ec3e21ab1ef7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 4: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 5: `using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;`
- Line 6: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 7: `using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;`
- Line 8: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 9: `using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;`
- Line 10: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L21): `public static class TeacherEndpoints`
- [Line 23](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L23): `public static RouteGroupBuilder MapTeacherEndpoints(this RouteGroupBuilder api)`
- [Line 78](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L78): `private static async Task<IResult> ListTeachersAsync(Dispatcher dispatcher, CancellationToken ct, bool includeArchived = false)`
- [Line 84](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L84): `private static async Task<IResult> GetTeacherAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)`
- [Line 90](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L90): `private static async Task<IResult> CreateTeacherAsync(CreateTeacherRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 97](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L97): `private static async Task<IResult> UpdateTeacherAsync(Guid id, UpdateTeacherRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 104](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L104): `private static async Task<IResult> SetTeacherAccountAsync(Guid id, SetTeacherAccountRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 111](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L111): `private static async Task<IResult> ArchiveTeacherAsync(Guid id, TeacherVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 117](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L117): `private static async Task<IResult> RestoreTeacherAsync(Guid id, TeacherVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 125](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L125): `public sealed record TeacherListResponse(IReadOnlyList<TeacherDto> Items);`
- [Line 128](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L128): `public sealed record CreateTeacherRequest(string FullName, string? Phone);`
- [Line 130](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L130): `public sealed record CreateTeacherResponse(Guid Id);`
- [Line 138](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L138): `public sealed record UpdateTeacherRequest`
- [Line 148](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L148): `public sealed record SetTeacherAccountRequest`
- [Line 156](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L156): `public sealed record TeacherVersionRequest`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 69 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
