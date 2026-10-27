# `tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs`

## Purpose

Declares `GetTeacherHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,398 bytes. Source hash: `fafe9e5d0a8a6245fa0930e358628155081ad0a0c281cda25daef0e1f087b08b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs#L9): `public sealed class GetTeacherHandlerTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs#L12): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs#L25): `public async Task HandleAsync_KnownId_ReturnsDto()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetTeacherHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var readService = new FakeTeacherReadService();
        var handler = new GetTeacherHandler(readService);

        var result = await handler.HandleAsync(new GetTeacherQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_KnownId_ReturnsDto()
    {
        var teacherId = Guid.CreateVersion7();
        var readService = new FakeTeacherReadService();
        readService.Teachers.Add(new TeacherDto(teacherId, "Amina Hassan", null, TeacherStatus.Active, null, null, 0, 0));
        var handler = new GetTeacherHandler(readService);

        var result = await handler.HandleAsync(new GetTeacherQuery(teacherId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Amina Hassan", result.Value.FullName);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
