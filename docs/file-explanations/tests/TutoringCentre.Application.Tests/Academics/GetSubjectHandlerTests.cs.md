# `tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs`

## Purpose

Declares `GetSubjectHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,371 bytes. Source hash: `503a9a0a8cf7f84593ad1c746550b39df7a44712974238811d7480168e501c70`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs#L9): `public sealed class GetSubjectHandlerTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs#L12): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs#L25): `public async Task HandleAsync_KnownId_ReturnsDto()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetSubjectHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var readService = new FakeSubjectReadService();
        var handler = new GetSubjectHandler(readService);

        var result = await handler.HandleAsync(new GetSubjectQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_KnownId_ReturnsDto()
    {
        var subjectId = Guid.CreateVersion7();
        var readService = new FakeSubjectReadService();
        readService.Subjects.Add(new SubjectDto(subjectId, "Mathematics", SubjectStatus.Active, 0));
        var handler = new GetSubjectHandler(readService);

        var result = await handler.HandleAsync(new GetSubjectQuery(subjectId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Mathematics", result.Value.Name);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
