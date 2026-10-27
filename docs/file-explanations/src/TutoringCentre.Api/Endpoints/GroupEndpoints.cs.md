# `src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Group endpoints (Day 45 contract). Each endpoint: bind → dispatch → map the result. No rule, repository, entity, tenant, role or scope logic: one route serves owners, secretaries and teachers alike, and the query handlers apply the actor's teacher scope below this layer, so there is no "my groups" route to bypass. Name, capacity and teacher are separate sub-resources with their own request models; none of them accepts a subject ID, so the subject fixed at creation cannot be changed through any route. Protected by the authorization fallback policy; none is anonymous.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 200 lines and 9,666 bytes. Source hash: `d7129ef56691191308533513a95d47c429e605afc54124f909f352f5fa6877ff`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 4: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 5: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 6: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 7: `using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;`
- Line 8: `using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;`
- Line 9: `using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;`
- Line 10: `using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;`
- Line 11: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L23): `public static class GroupEndpoints`
- [Line 25](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L25): `public static RouteGroupBuilder MapGroupEndpoints(this RouteGroupBuilder api)`
- [Line 93](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L93): `private static async Task<IResult> ListGroupsAsync(`
- [Line 101](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L101): `private static async Task<IResult> GetGroupAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)`
- [Line 107](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L107): `private static async Task<IResult> CreateGroupAsync(CreateGroupRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 114](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L114): `private static async Task<IResult> RenameGroupAsync(Guid id, RenameGroupRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 120](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L120): `private static async Task<IResult> ChangeGroupCapacityAsync(`
- [Line 128](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L128): `private static async Task<IResult> AssignGroupTeacherAsync(`
- [Line 136](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L136): `private static async Task<IResult> ArchiveGroupAsync(Guid id, GroupVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 142](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L142): `private static async Task<IResult> RestoreGroupAsync(Guid id, GroupVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 150](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L150): `public sealed record GroupListResponse(IReadOnlyList<GroupDto> Items);`
- [Line 155](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L155): `/// handler. required on the value types: a positional-record Guid or int silently defaults when the property is`
- [Line 159](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L159): `public sealed record CreateGroupRequest`
- [Line 170](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L170): `public sealed record CreateGroupResponse(Guid Id);`
- [Line 173](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L173): `public sealed record RenameGroupRequest`
- [Line 181](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L181): `public sealed record ChangeGroupCapacityRequest`
- [Line 189](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L189): `public sealed record AssignGroupTeacherRequest`
- [Line 197](../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L197): `public sealed record GroupVersionRequest`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/ActorMiddleware.cs:34](../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L34)
- [src/TutoringCentre.Api/Auth/SelectCentreRequest.cs:7](../../../../../src/TutoringCentre.Api/Auth/SelectCentreRequest.cs#L7)
- [src/TutoringCentre.Api/Auth/SessionClaims.cs:23](../../../../../src/TutoringCentre.Api/Auth/SessionClaims.cs#L23)
- [src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs:27](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L27)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:203](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L203)
- [src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs:35](../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs#L35)
- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:77](../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L77)
- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:63](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L63)
- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:81](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L81)
- [src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs:80](../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L80)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:113](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L113)
- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:74](../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L74)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:84](../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L84)
- [src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs:24](../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs#L24)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs:7](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentResult.cs:3](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentResult.cs#L3)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs:7](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentResult.cs:4](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentResult.cs#L4)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs:7](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs:18](../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs#L18)
- 354 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 110 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
