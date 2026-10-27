# `tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs`

## Purpose

Declares `ListTeachersHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,365 bytes. Source hash: `78dc77760c2e9f8a074ae5ebcee9b8bf06010f8b4a66b1f12c6f6f6b2028d562`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs#L8): `public sealed class ListTeachersHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs#L13): `public async Task HandleAsync_PassesIncludeArchivedFlagThrough(bool includeArchived)`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs#L24): `public async Task HandleAsync_ReturnsWhatTheReadServiceProvides()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListTeachersHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleAsync_PassesIncludeArchivedFlagThrough(bool includeArchived)
    {
        var readService = new FakeTeacherReadService();
        var handler = new ListTeachersHandler(readService);

        await handler.HandleAsync(new ListTeachersQuery(includeArchived), CancellationToken.None);

        Assert.Equal(includeArchived, readService.LastIncludeArchivedRequested);
    }

    [Fact]
    public async Task HandleAsync_ReturnsWhatTheReadServiceProvides()
    {
        var readService = new FakeTeacherReadService();
        readService.Teachers.Add(new TeacherDto(Guid.CreateVersion7(), "Amina Hassan", null, TeacherStatus.Active, null, null, 0, 0));
        var handler = new ListTeachersHandler(readService);

        var result = await handler.HandleAsync(new ListTeachersQuery(false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Amina Hassan", result.Value[0].FullName);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
