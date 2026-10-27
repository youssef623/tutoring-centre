# `tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory subject repository. `Existing` simulates stored rows; `Added` records Add calls.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 1,084 bytes. Source hash: `4e77cb0beebdcf31a0e605960889e28936960ecf6ebaf280ebc6d703ec2f8e46`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs#L7): `public sealed class FakeSubjectRepository : ISubjectRepository`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs#L13): `public Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs#L16): `public Task<Subject?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs#L19): `public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>`
- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs#L23): `public void Add(Subject subject) => Added.Add(subject);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs:128](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L128)
- [tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs:20](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L20)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:141](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L141)
- [tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs:79](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L79)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:167](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L167)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:107](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L107)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory subject repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeSubjectRepository : ISubjectRepository
{
    public List<Subject> Existing { get; } = [];

    public List<Subject> Added { get; } = [];

    public Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(subject => subject.Id == id));

    public Task<Subject?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(subject => subject.Id == id));

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(subject =>
            subject.NormalizedName == normalizedName && subject.Id != excludingId));

    public void Add(Subject subject) => Added.Add(subject);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
