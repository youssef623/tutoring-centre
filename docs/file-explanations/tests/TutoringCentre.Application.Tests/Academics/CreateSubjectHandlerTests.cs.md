# `tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs`

## Purpose

Declares `CreateSubjectHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 72 lines and 2,900 bytes. Source hash: `2272474426cc21020eb17dc9de9b88d9bea9b94a284ff1da9f28b4fe2e7ecf0f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L10): `public sealed class CreateSubjectHandlerTests`
- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L15): `public async Task HandleAsync_Valid_AddsSubjectWithActorsCentreAndReturnsId()`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L35): `public async Task HandleAsync_DuplicateDifferingByCaseAndSpaces_ReturnsNameTakenAndAddsNothing()`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L56): `public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_AddsSubjectWithActorsCentreAndReturnsId()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeSubjectRepository();
        var handler = new CreateSubjectHandler(actorContext, repository);

        // Act
        var result = await handler.HandleAsync(new CreateSubjectCommand("Mathematics"), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal(NileCentreId, repository.Added[0].CentreId);
        Assert.Equal("Mathematics", repository.Added[0].Name);
        Assert.Equal(repository.Added[0].Id, result.Value.SubjectId);
    }

    [Fact]
    public async Task HandleAsync_DuplicateDifferingByCaseAndSpaces_ReturnsNameTakenAndAddsNothing()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(Subject.Create(NileCentreId, "Mathematics").Value);
        var handler = new CreateSubjectHandler(actorContext, repository);

        // Act
        var result = await handler.HandleAsync(new CreateSubjectCommand("  mathematics "), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("name", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeSubjectRepository();
        var handler = new CreateSubjectHandler(actorContext, repository);

        // Act
        var result = await handler.HandleAsync(new CreateSubjectCommand("   "), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
