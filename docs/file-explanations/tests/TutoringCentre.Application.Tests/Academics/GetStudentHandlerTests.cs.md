# `tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs`

## Purpose

Declares `GetStudentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,245 bytes. Source hash: `01ab5a6c5f467b50de295fb6108b25f578c64d60adec60ba1042e32334ef17c4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students;`
- Line 2: `using TutoringCentre.Application.Academics.Students.Queries.GetStudent;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs#L8): `public sealed class GetStudentHandlerTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs#L11): `public async Task HandleAsync_Known_ReturnsDetail()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs#L25): `public async Task HandleAsync_Unknown_ReturnsStudentNotFound()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetStudentHandlerTests
{
    [Fact]
    public async Task HandleAsync_Known_ReturnsDetail()
    {
        var readService = new FakeStudentReadService();
        var detail = new StudentDetailDto(Guid.CreateVersion7(), "Youssef Omar", null, null, StudentStatus.Active, 0, []);
        readService.Details.Add(detail);
        var handler = new GetStudentHandler(readService);

        var result = await handler.HandleAsync(new GetStudentQuery(detail.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(detail, result.Value);
    }

    [Fact]
    public async Task HandleAsync_Unknown_ReturnsStudentNotFound()
    {
        var handler = new GetStudentHandler(new FakeStudentReadService());

        var result = await handler.HandleAsync(new GetStudentQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
