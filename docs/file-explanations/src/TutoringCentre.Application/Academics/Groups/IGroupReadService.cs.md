# `src/TutoringCentre.Application/Academics/Groups/IGroupReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Every method takes a `TeacherScope` and there is no overload without one, so a new read cannot forget the ownership guard without a compile error. The scope is applied in SQL: out-of-scope rows never leave the database.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,604 bytes. Source hash: `79c4459c729a379e2c0f20bd8c15a9533d0b4bc89e8006827d936750d28d4e20`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupReadService.cs#L11): `public interface IGroupReadService`
- [Line 21](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupReadService.cs#L21): `public sealed record GroupListCriteria(Guid? SubjectId, Guid? TeacherId, bool IncludeArchived);`
- [Line 24](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupReadService.cs#L24): `public sealed record GroupDto(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs#L16)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:134](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L134)
- [src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs:17](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L17)
- [src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs:21](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L21)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:158](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L158)
- [tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs:26](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L26)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs:11](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs#L11)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs:15](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups;

/// <summary>
/// Read-side port: returns DTO-shaped data directly. Every method takes a <see cref="TeacherScope"/> and there
/// is no overload without one, so a new read cannot forget the ownership guard without a compile error. The
/// scope is applied in SQL: out-of-scope rows never leave the database.
/// </summary>
public interface IGroupReadService
{
    /// <summary>Groups ordered by normalized name, scoped to the acting centre and to <paramref name="scope"/>. Capped at 500 rows; there is no paging.</summary>
    Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct);

    /// <summary>One group by ID, or null when it does not exist, is not visible to the acting centre, or is outside <paramref name="scope"/>.</summary>
    Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct);
}

/// <summary>Optional filters for the group list. Intersected with the scope, never widening it.</summary>
public sealed record GroupListCriteria(Guid? SubjectId, Guid? TeacherId, bool IncludeArchived);

/// <summary>The one shape every group read returns. <see cref="EnrolledCount"/> is the group's active enrollments, counted when read.</summary>
public sealed record GroupDto(
    Guid Id,
    string Name,
    Guid SubjectId,
    string SubjectName,
    Guid TeacherId,
    string TeacherName,
    int Capacity,
    int EnrolledCount,
    GroupStatus Status,
    uint Version);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
