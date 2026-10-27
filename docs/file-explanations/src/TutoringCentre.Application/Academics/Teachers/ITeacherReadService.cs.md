# `src/TutoringCentre.Application/Academics/Teachers/ITeacherReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Never exposes the Teacher entity or EF types.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 30 lines and 1,364 bytes. Source hash: `02daeb70584d4f3b1fc2d94d1e794aebd2cee6270bd3d502644f663bc43d11fa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherReadService.cs#L6): `public interface ITeacherReadService`
- [Line 22](../../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherReadService.cs#L22): `public sealed record TeacherDto(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:132](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L132)
- [src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs:18](../../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L18)
- [src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs:17](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:230](../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L230)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs:7](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Teachers;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Teacher entity or EF types.</summary>
public interface ITeacherReadService
{
    /// <summary>Teachers ordered by search name, scoped to the acting centre. Capped at 500 rows; there is no paging.</summary>
    Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct);

    /// <summary>One teacher by ID, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct);

    /// <summary>The ID of the active teacher linked to this user in the acting centre, or null when none is linked or the link belongs to an archived teacher.</summary>
    Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct);
}

/// <summary>
/// The one shape every teacher read returns. <see cref="ActiveGroupCount"/> counts the teacher's active groups. The
/// linked account, if any, is shown by display name only — never email, which stays behind <c>staff.view</c>.
/// </summary>
public sealed record TeacherDto(
    Guid Id,
    string FullName,
    string? Phone,
    TeacherStatus Status,
    Guid? UserId,
    string? UserDisplayName,
    int ActiveGroupCount,
    uint Version);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
