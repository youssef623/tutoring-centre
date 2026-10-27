# `tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs`

## Purpose

Declares `LookupParentByPhoneHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 1,818 bytes. Source hash: `1c60c40fd6a6392813a0c3ca7921ca84fb068653f8364526233f47fbb6bc5c97`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents;`
- Line 2: `using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L8): `public sealed class LookupParentByPhoneHandlerTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L11): `public async Task HandleAsync_FoundByDifferentSpelling_ReturnsTheParent()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L25): `public async Task HandleAsync_NoParentWithThisPhone_SucceedsWithNull()`
- [Line 37](../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L37): `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class LookupParentByPhoneHandlerTests
{
    [Fact]
    public async Task HandleAsync_FoundByDifferentSpelling_ReturnsTheParent()
    {
        var existing = new ParentDto(Guid.CreateVersion7(), "Omar Hassan", "+201012345678", 0);
        var readService = new FakeParentReadService();
        readService.ByPhone.Add(existing);
        var handler = new LookupParentByPhoneHandler(readService);

        var result = await handler.HandleAsync(new LookupParentByPhoneQuery("0101 234 5678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(existing, result.Value.Parent);
    }

    [Fact]
    public async Task HandleAsync_NoParentWithThisPhone_SucceedsWithNull()
    {
        var readService = new FakeParentReadService();
        var handler = new LookupParentByPhoneHandler(readService);

        var result = await handler.HandleAsync(new LookupParentByPhoneQuery("01012345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Parent);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()
    {
        var readService = new FakeParentReadService();
        var handler = new LookupParentByPhoneHandler(readService);

        var result = await handler.HandleAsync(new LookupParentByPhoneQuery("12345"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Contains("phone", result.Error.Fields!.Keys);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
