# `tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs`

## Purpose

Declares `UpdateParentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 90 lines and 3,763 bytes. Source hash: `eb2650b570fb79d2bdba0acb94f958f246a3d3450c916157cb5602e6159eaee9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L8): `public sealed class UpdateParentHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L27): `public async Task HandleAsync_SamePhone_SucceedsWithoutRunningAUniquenessQuery()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L43): `public async Task HandleAsync_PhoneChangedAndTakenByAnother_ReturnsPhoneTaken()`
- [Line 62](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L62): `public async Task HandleAsync_PhoneChangedToAFreeOne_Succeeds()`
- [Line 77](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L77): `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateParentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeParentRepository();
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(Guid.CreateVersion7(), "Omar Hassan", "01012345678", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SamePhone_SucceedsWithoutRunningAUniquenessQuery()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(parent.Id, "Omar Hassan Ali", "01012345678", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Omar Hassan Ali", parent.FullName);
        Assert.Equal(0, repository.PhoneExistsCallCount);
    }

    [Fact]
    public async Task HandleAsync_PhoneChangedAndTakenByAnother_ReturnsPhoneTaken()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var other = Parent.Create(NileCentreId, "Amal Fathy", PhoneNumber.Create("01212345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        repository.Existing.Add(other);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(parent.Id, "Omar Hassan", "01212345678", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(1, repository.PhoneExistsCallCount);
    }

    [Fact]
    public async Task HandleAsync_PhoneChangedToAFreeOne_Succeeds()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(parent.Id, "Omar Hassan", "01112345678", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("+201112345678", parent.Phone.Value);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(new UpdateParentCommand(parent.Id, "Omar Hassan", "12345", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Contains("phone", result.Error.Fields!.Keys);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
