# `tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory student repository. `Existing` simulates stored rows; `Added` records Add calls.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 831 bytes. Source hash: `b4ead4849031b278aec59aaa0c0bbf769b7ceb75538e725a976091be845a2c48`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs#L7): `public sealed class FakeStudentRepository : IStudentRepository`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs#L13): `public Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs#L16): `public Task<Student?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs#L19): `public void Add(Student student) => Added.Add(student);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:135](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L135)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:131](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L131)
- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs:136](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L136)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:151](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L151)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory student repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeStudentRepository : IStudentRepository
{
    public List<Student> Existing { get; } = [];

    public List<Student> Added { get; } = [];

    public Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(student => student.Id == id));

    public Task<Student?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(student => student.Id == id));

    public void Add(Student student) => Added.Add(student);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
