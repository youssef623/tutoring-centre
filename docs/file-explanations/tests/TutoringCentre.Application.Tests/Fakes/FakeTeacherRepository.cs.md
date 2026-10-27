# `tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory teacher repository. `Existing` simulates stored rows; `Added` records Add calls.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 1,072 bytes. Source hash: `78d21338bcda15ae62ce01cc9e36da79365fe66e518f9b9e93d24cfcc11f8210`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs#L7): `public sealed class FakeTeacherRepository : ITeacherRepository`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs#L13): `public Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs#L16): `public Task<Teacher?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs#L19): `public Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct) =>`
- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs#L23): `public void Add(Teacher teacher) => Added.Add(teacher);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:93](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L93)
- [tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs:129](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L129)
- [tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs:18](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:142](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L142)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs:83](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L83)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs:155](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L155)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:214](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L214)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory teacher repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeTeacherRepository : ITeacherRepository
{
    public List<Teacher> Existing { get; } = [];

    public List<Teacher> Added { get; } = [];

    public Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(teacher => teacher.Id == id));

    public Task<Teacher?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(teacher => teacher.Id == id));

    public Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(teacher =>
            teacher.UserId == userId && teacher.Id != excludingTeacherId));

    public void Add(Teacher teacher) => Added.Add(teacher);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
