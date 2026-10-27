# `tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs`

## Purpose

Declares `ListStudentsHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 941 bytes. Source hash: `5c368f512ff9b880bc33a82e18e2a9d4db65a18383f98daddd750d76f25e5bf8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students;`
- Line 2: `using TutoringCentre.Application.Academics.Students.Queries.ListStudents;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs#L8): `public sealed class ListStudentsHandlerTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs#L11): `public async Task HandleAsync_PassesCriteriaThroughAndReturnsThePage()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListStudentsHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesCriteriaThroughAndReturnsThePage()
    {
        var readService = new FakeStudentReadService();
        var item = new StudentListItemDto(Guid.CreateVersion7(), "Youssef Omar", null, StudentStatus.Active, null, null);
        readService.Page = new StudentPageDto([item], "next-cursor");
        var handler = new ListStudentsHandler(readService);

        var result = await handler.HandleAsync(new ListStudentsQuery("omar", false, "cursor", 25), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(readService.Page, result.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
