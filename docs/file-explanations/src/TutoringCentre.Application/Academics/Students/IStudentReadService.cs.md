# `src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Never exposes the Student entity or EF types.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,696 bytes. Source hash: `c57f35be92b5d1d7601fb3914696884bffebbfdd3f99a556a684965e0e57a586`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs#L6): `public interface IStudentReadService`
- [Line 16](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs#L16): `public sealed record StudentSearchCriteria(string? Search, bool IncludeArchived, string? Cursor, int PageSize);`
- [Line 19](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs#L19): `public sealed record StudentListItemDto(`
- [Line 22](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs#L22): `public sealed record StudentPageDto(IReadOnlyList<StudentListItemDto> Items, string? NextCursor);`
- [Line 25](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs#L25): `public sealed record StudentDetailDto(`
- [Line 28](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentReadService.cs#L28): `public sealed record StudentParentDto(Guid ParentId, string FullName, string Phone, ParentRelationship Relationship, bool IsPrimary);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs#L8)
- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs:12](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs#L12)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:140](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L140)
- [src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs:18](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L18)
- [src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs:20](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L20)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:165](../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L165)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs:6](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs#L6)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs:12](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs:37](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs#L37)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Student entity or EF types.</summary>
public interface IStudentReadService
{
    /// <summary>A page of students matching the search criteria, scoped to the acting centre.</summary>
    Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct);

    /// <summary>One student with their parents, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct);
}

/// <summary>What the list query passes to the read service. No centre: scope comes from the actor.</summary>
public sealed record StudentSearchCriteria(string? Search, bool IncludeArchived, string? Cursor, int PageSize);

/// <summary>One row of the student list, with the primary parent's contact details for display.</summary>
public sealed record StudentListItemDto(
    Guid Id, string FullName, string? SchoolYear, StudentStatus Status, string? PrimaryParentName, string? PrimaryParentPhone);

public sealed record StudentPageDto(IReadOnlyList<StudentListItemDto> Items, string? NextCursor);

/// <summary>A student plus every linked parent, for the student detail page. Parents list the primary first.</summary>
public sealed record StudentDetailDto(
    Guid Id, string FullName, string? Phone, string? SchoolYear, StudentStatus Status, uint Version, IReadOnlyList<StudentParentDto> Parents);

public sealed record StudentParentDto(Guid ParentId, string FullName, string Phone, ParentRelationship Relationship, bool IsPrimary);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
