# `tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for enrollment reads. Each row carries its group's teacher, so the scope is applied the way the real adapter applies it (rows outside it are invisible); records the last scope it was asked for.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 44 lines and 1,781 bytes. Source hash: `ed6af4f8a175471eeff67742ee82b5992cf04df9b1ef3fe4e41aa18841bc817c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 2: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs#L10): `public sealed class FakeEnrollmentReadService : IEnrollmentReadService`
- [Line 20](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs#L20): `public Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct)`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs#L30): `public Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:133](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L133)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs:19](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for enrollment reads. Each row carries its group's teacher, so the scope is applied the way
/// the real adapter applies it (rows outside it are invisible); records the last scope it was asked for.
/// </summary>
public sealed class FakeEnrollmentReadService : IEnrollmentReadService
{
    public List<(Guid TeacherId, GroupRosterDto Roster)> Rosters { get; } = [];

    public HashSet<Guid> StudentIds { get; } = [];

    public List<(Guid StudentId, Guid TeacherId, StudentEnrollmentDto Enrollment)> StudentEnrollments { get; } = [];

    public TeacherScope? LastScope { get; private set; }

    public Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        var match = Rosters
            .Where(row => row.Roster.GroupId == groupId && scope.Allows(row.TeacherId))
            .Select(row => row.Roster)
            .FirstOrDefault();
        return Task.FromResult(match);
    }

    public Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        if (!StudentIds.Contains(studentId))
        {
            return Task.FromResult<IReadOnlyList<StudentEnrollmentDto>?>(null);
        }

        IReadOnlyList<StudentEnrollmentDto> visible = StudentEnrollments
            .Where(row => row.StudentId == studentId && scope.Allows(row.TeacherId))
            .Select(row => row.Enrollment)
            .ToList();
        return Task.FromResult<IReadOnlyList<StudentEnrollmentDto>?>(visible);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
