# `tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for teacher reads. Records the last includeArchived flag it was asked for, so a handler test can assert it was passed through.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 1,110 bytes. Source hash: `c474fd5fff645b48b90c9333a80ec544191393a3a68af63f2016e1f1dc94cb49`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs#L7): `public sealed class FakeTeacherReadService : ITeacherReadService`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs#L13): `public Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct)`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs#L19): `public Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct) =>`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs#L22): `public Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs:14](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs#L14)
- [tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:215](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L215)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for teacher reads. Records the last <c>includeArchived</c> flag it was asked for, so a handler test can assert it was passed through.</summary>
public sealed class FakeTeacherReadService : ITeacherReadService
{
    public List<TeacherDto> Teachers { get; } = [];

    public bool? LastIncludeArchivedRequested { get; private set; }

    public Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct)
    {
        LastIncludeArchivedRequested = includeArchived;
        return Task.FromResult<IReadOnlyList<TeacherDto>>(Teachers);
    }

    public Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Teachers.FirstOrDefault(teacher => teacher.Id == id));

    public Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct) =>
        Task.FromResult(Teachers.FirstOrDefault(teacher => teacher.UserId == userId && teacher.Status == TeacherStatus.Active)?.Id);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
