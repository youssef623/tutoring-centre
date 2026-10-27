# `tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs`

## Purpose

Declares `GetParentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,309 bytes. Source hash: `d4422d5e0d7ebda67f41072666266c357179355102a37c08c7c9905bc2257ea1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents;`
- Line 2: `using TutoringCentre.Application.Academics.Parents.Queries.GetParent;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs#L8): `public sealed class GetParentHandlerTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs#L11): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs#L24): `public async Task HandleAsync_KnownId_ReturnsDetail()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetParentHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var readService = new FakeParentReadService();
        var handler = new GetParentHandler(readService);

        var result = await handler.HandleAsync(new GetParentQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_KnownId_ReturnsDetail()
    {
        var detail = new ParentDetailDto(Guid.CreateVersion7(), "Omar Hassan", "+201012345678", 0, []);
        var readService = new FakeParentReadService();
        readService.Details.Add(detail);
        var handler = new GetParentHandler(readService);

        var result = await handler.HandleAsync(new GetParentQuery(detail.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(detail, result.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
