# `src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Subject endpoints (Day 26 contract: docs/architecture/api-conventions.md). Each endpoint: bind → dispatch → map the result. No rule, repository, entity or tenant logic — the tenant comes only from the actor, through the dispatcher's own layer 1 step. Protected by the authorization fallback policy; none is anonymous.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 129 lines and 6,443 bytes. Source hash: `c0d3d72aa791c38a4381e1690497b139429dac2f6df87d1f099506734e1fbfae`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects;`
- Line 3: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 5: `using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;`
- Line 6: `using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;`
- Line 7: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 8: `using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;`
- Line 9: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L18): `public static class SubjectEndpoints`
- [Line 20](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L20): `public static RouteGroupBuilder MapSubjectEndpoints(this RouteGroupBuilder api)`
- [Line 68](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L68): `private static async Task<IResult> ListSubjectsAsync(Dispatcher dispatcher, CancellationToken ct, bool includeArchived = false)`
- [Line 74](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L74): `private static async Task<IResult> GetSubjectAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)`
- [Line 80](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L80): `private static async Task<IResult> CreateSubjectAsync(CreateSubjectRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 86](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L86): `private static async Task<IResult> RenameSubjectAsync(Guid id, RenameSubjectRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 92](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L92): `private static async Task<IResult> ArchiveSubjectAsync(Guid id, SubjectVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 98](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L98): `private static async Task<IResult> RestoreSubjectAsync(Guid id, SubjectVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 106](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L106): `public sealed record SubjectListResponse(IReadOnlyList<SubjectDto> Items);`
- [Line 109](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L109): `public sealed record CreateSubjectRequest(string Name);`
- [Line 111](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L111): `public sealed record CreateSubjectResponse(Guid Id);`
- [Line 118](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L118): `public sealed record RenameSubjectRequest`
- [Line 126](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L126): `public sealed record SubjectVersionRequest`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 39 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
