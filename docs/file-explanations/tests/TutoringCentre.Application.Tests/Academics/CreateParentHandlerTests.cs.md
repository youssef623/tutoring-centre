# `tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs`

## Purpose

Declares `CreateParentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 80 lines and 3,617 bytes. Source hash: `547ff94037ae6609fbb041fd813c31986ed427cf79b87ee79155dcd0e64f6e49`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L10): `public sealed class CreateParentHandlerTests`
- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L15): `public async Task HandleAsync_Valid_AddsParentWithActorsCentreAndCanonicalPhone()`
- [Line 32](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L32): `public async Task HandleAsync_DifferentlySpelledPhoneAlreadyExists_ReturnsPhoneTakenAndAddsNothing()`
- [Line 50](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L50): `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithFieldAndAddsNothing()`
- [Line 67](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L67): `public async Task HandleAsync_OneCharacterName_ReturnsNameInvalidAndAddsNothing()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateParentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_AddsParentWithActorsCentreAndCanonicalPhone()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("Omar Hassan", "01012345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal(NileCentreId, repository.Added[0].CentreId);
        Assert.Equal("+201012345678", repository.Added[0].Phone.Value);
        Assert.Equal(repository.Added[0].Id, result.Value.ParentId);
    }

    [Fact]
    public async Task HandleAsync_DifferentlySpelledPhoneAlreadyExists_ReturnsPhoneTakenAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        repository.Existing.Add(Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("+201012345678").Value).Value);
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("Another Parent", "0101 234 5678"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithFieldAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("Omar Hassan", "12345"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_OneCharacterName_ReturnsNameInvalidAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("O", "01012345678"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.name_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
