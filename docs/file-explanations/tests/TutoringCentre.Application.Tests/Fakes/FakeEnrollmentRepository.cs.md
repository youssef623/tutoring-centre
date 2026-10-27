# `tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory enrollment repository. `Existing` simulates stored rows; `Added` records Add calls.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 1,427 bytes. Source hash: `12606ccde6042bf3cafaa791e1b16d9b93d3558becaf131ac8b5d3c1eb349f5d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L7): `public sealed class FakeEnrollmentRepository : IEnrollmentRepository`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L13): `public Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L16): `public Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct) =>`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L19): `public Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct) =>`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L22): `public Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct) =>`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L25): `public void Add(Enrollment enrollment) => Added.Add(enrollment);`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L27): `private IEnumerable<Enrollment> Active() =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:19](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:137](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L137)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:132](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L132)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:137](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L137)
- [tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory enrollment repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeEnrollmentRepository : IEnrollmentRepository
{
    public List<Enrollment> Existing { get; } = [];

    public List<Enrollment> Added { get; } = [];

    public Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(enrollment => enrollment.Id == id));

    public Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct) =>
        Task.FromResult(Active().Any(enrollment => enrollment.StudentId == studentId && enrollment.GroupId == groupId));

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct) =>
        Task.FromResult(Active().Any(enrollment => enrollment.GroupId == groupId));

    public Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct) =>
        Task.FromResult(Active().Any(enrollment => enrollment.StudentId == studentId));

    public void Add(Enrollment enrollment) => Added.Add(enrollment);

    private IEnumerable<Enrollment> Active() =>
        Existing.Concat(Added).Where(enrollment => enrollment.Status == EnrollmentStatus.Active);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
