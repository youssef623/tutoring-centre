# `tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for student reads.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 641 bytes. Source hash: `91508451c65bff86198a7f24839cd90c99c051de51447776eb1728ac983fd8da`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs#L6): `public sealed class FakeStudentReadService : IStudentReadService`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs#L12): `public Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct) => Task.FromResult(Page);`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs#L14): `public Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs:13](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs#L13)
- [tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs:13](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs#L13)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:152](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L152)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for student reads.</summary>
public sealed class FakeStudentReadService : IStudentReadService
{
    public List<StudentDetailDto> Details { get; } = [];

    public StudentPageDto Page { get; set; } = new([], null);

    public Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct) => Task.FromResult(Page);

    public Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Details.FirstOrDefault(student => student.Id == id));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
