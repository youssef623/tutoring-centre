# `src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Student endpoints (Day 41 contract: docs/architecture/api-conventions.md). Each endpoint: bind → dispatch → map the result. No rule, repository, entity or tenant logic — the tenant comes only from the actor, through the dispatcher's own layer 1 step. Protected by the authorization fallback policy; none is anonymous. Unlink and make-primary are action sub-resources carrying the student's version in the body, following Month 2's convention for state changes, instead of an HTTP DELETE with a body: the parent id comes from the route and the version from the body, so the two cannot disagree about which student is being changed.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 224 lines and 11,125 bytes. Source hash: `5888cee7f7c07b5d3b7643e8878fc2e4329eb6af6ebd148f7b0ecddc05ccc54d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Students;`
- Line 3: `using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;`
- Line 4: `using TutoringCentre.Application.Academics.Students.Commands.LinkParent;`
- Line 5: `using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;`
- Line 6: `using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;`
- Line 7: `using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;`
- Line 8: `using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;`
- Line 9: `using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;`
- Line 10: `using TutoringCentre.Application.Academics.Students.Queries.GetStudent;`
- Line 11: `using TutoringCentre.Application.Academics.Students.Queries.ListStudents;`
- Line 12: `using TutoringCentre.Application.Common.Cqrs;`
- Line 13: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L25): `public static class StudentEndpoints`
- [Line 29](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L29): `public static RouteGroupBuilder MapStudentEndpoints(this RouteGroupBuilder api)`
- [Line 105](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L105): `private static async Task<IResult> ListStudentsAsync(`
- [Line 113](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L113): `private static async Task<IResult> GetStudentAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)`
- [Line 119](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L119): `private static async Task<IResult> RegisterStudentAsync(RegisterStudentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 127](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L127): `private static async Task<IResult> UpdateStudentAsync(Guid id, UpdateStudentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 134](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L134): `private static async Task<IResult> ArchiveStudentAsync(Guid id, StudentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 140](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L140): `private static async Task<IResult> RestoreStudentAsync(Guid id, StudentVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 146](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L146): `private static async Task<IResult> LinkStudentParentAsync(Guid id, LinkStudentParentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 153](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L153): `private static async Task<IResult> UnlinkStudentParentAsync(`
- [Line 160](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L160): `private static async Task<IResult> MakeStudentParentPrimaryAsync(`
- [Line 169](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L169): `public sealed record RegisterStudentRequestParent`
- [Line 179](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L179): `public sealed record RegisterStudentRequest`
- [Line 192](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L192): `public sealed record RegisterStudentResponse(Guid Id, Guid ParentId);`
- [Line 199](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L199): `public sealed record UpdateStudentRequest`
- [Line 211](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L211): `public sealed record StudentVersionRequest`
- [Line 217](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L217): `public sealed record LinkStudentParentRequest`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 134 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
