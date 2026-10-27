# `tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for subject reads. Records the last includeArchived flag it was asked for, so a handler test can assert it was passed through.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 858 bytes. Source hash: `990b7ce0362cf8bf0f8f5e9c94ef937064eb250dd8a8eb9ca6c041074d5577d1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs#L6): `public sealed class FakeSubjectReadService : ISubjectReadService`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs#L12): `public Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct)`
- [Line 18](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs#L18): `public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs:14](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs#L14)
- [tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:168](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L168)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:108](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L108)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for subject reads. Records the last <c>includeArchived</c> flag it was asked for, so a handler test can assert it was passed through.</summary>
public sealed class FakeSubjectReadService : ISubjectReadService
{
    public List<SubjectDto> Subjects { get; } = [];

    public bool? LastIncludeArchivedRequested { get; private set; }

    public Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct)
    {
        LastIncludeArchivedRequested = includeArchived;
        return Task.FromResult<IReadOnlyList<SubjectDto>>(Subjects);
    }

    public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Subjects.FirstOrDefault(subject => subject.Id == id));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
