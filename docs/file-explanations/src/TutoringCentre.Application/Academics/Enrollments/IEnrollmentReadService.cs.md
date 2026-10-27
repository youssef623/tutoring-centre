# `src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Every method takes a `TeacherScope` and there is no overload without one: an enrollment is a group-owned resource, so the same ownership guard as groups applies, as a predicate on the group row in SQL. Out-of-scope rows never leave the database.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 43 lines and 2,050 bytes. Source hash: `3b6c9d494904f9ce28cea66439c7a87991e684d0f6405e1996ee39cf9ab12661`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs#L11): `public interface IEnrollmentReadService`
- [Line 29](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs#L29): `public sealed record GroupRosterDto(Guid GroupId, string GroupName, int Capacity, int EnrolledCount, IReadOnlyList<RosterEntryDto> Items);`
- [Line 31](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs#L31): `public sealed record RosterEntryDto(Guid EnrollmentId, Guid StudentId, string StudentName, DateTimeOffset EnrolledAt, uint Version);`
- [Line 33](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentReadService.cs#L33): `public sealed record StudentEnrollmentDto(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L14)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:143](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L143)
- [src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs:17](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:154](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L154)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs:10](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs#L10)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Enrollments;

/// <summary>
/// Read-side port: returns DTO-shaped data directly. Every method takes a <see cref="TeacherScope"/> and there is
/// no overload without one: an enrollment is a group-owned resource, so the same ownership guard as groups
/// applies, as a predicate on the group row in SQL. Out-of-scope rows never leave the database.
/// </summary>
public interface IEnrollmentReadService
{
    /// <summary>
    /// The group's active enrollments with student names, ordered by search name and capped at
    /// <see cref="Group.MaxCapacity"/>. Null when the group does not exist, is not visible to the acting centre,
    /// or is outside <paramref name="scope"/>.
    /// </summary>
    Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct);

    /// <summary>
    /// The student's enrollments in groups within <paramref name="scope"/>: active first, then newest first.
    /// Null when the student does not exist or is not visible to the acting centre; an empty list when the
    /// student exists but has no visible enrollment.
    /// </summary>
    Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct);
}

/// <summary>Who holds a seat in a group. Names only: no contact data of students or parents.</summary>
public sealed record GroupRosterDto(Guid GroupId, string GroupName, int Capacity, int EnrolledCount, IReadOnlyList<RosterEntryDto> Items);

public sealed record RosterEntryDto(Guid EnrollmentId, Guid StudentId, string StudentName, DateTimeOffset EnrolledAt, uint Version);

public sealed record StudentEnrollmentDto(
    Guid EnrollmentId,
    Guid GroupId,
    string GroupName,
    string SubjectName,
    string TeacherName,
    EnrollmentStatus Status,
    DateTimeOffset EnrolledAt,
    DateTimeOffset? EndedAt,
    EnrollmentEndReason? EndReason,
    uint Version);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
