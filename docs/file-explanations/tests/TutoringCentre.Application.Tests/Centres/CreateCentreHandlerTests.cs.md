# `tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs`

## Purpose

Declares `CreateCentreHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 92 lines and 3,783 bytes. Source hash: `fe2f2980725cdddf7780b6891dd58105c8469c5ab5a5982b686aa59432b30ce4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Centres;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L9): `public sealed class CreateCentreHandlerTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L12): `public async Task HandleAsync_AnonymousActor_ReturnsForbiddenAndDoesNotAdd()`
- [Line 32](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L32): `public async Task HandleAsync_SlugAlreadyExists_ReturnsConflictAndDoesNotAdd()`
- [Line 53](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L53): `public async Task HandleAsync_InvalidSlug_ReturnsDomainErrorAndDoesNotAdd()`
- [Line 72](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L72): `public async Task HandleAsync_SystemActorValidInput_AddsCentreAndReturnsResult()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class CreateCentreHandlerTests
{
    [Fact]
    public async Task HandleAsync_AnonymousActor_ReturnsForbiddenAndDoesNotAdd()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        var repository = new FakeCentreRepository();
        repository.Existing.Add(Centre.Create("Existing", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value);
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("centre.create_forbidden", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_SlugAlreadyExists_ReturnsConflictAndDoesNotAdd()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new SystemActor(null));
        var repository = new FakeCentreRepository();
        repository.Existing.Add(Centre.Create("Existing", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value);
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("centre.slug_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidSlug_ReturnsDomainErrorAndDoesNotAdd()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new SystemActor(null));
        var repository = new FakeCentreRepository();
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "Bad Slug", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("centre.slug_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_SystemActorValidInput_AddsCentreAndReturnsResult()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new SystemActor(null));
        var repository = new FakeCentreRepository();
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal("Nile Tutoring Centre", repository.Added[0].Name);
        Assert.Equal("nile-centre", repository.Added[0].Slug);
        Assert.Equal("nile-centre", result.Value.Slug);
        Assert.Equal(repository.Added[0].Id, result.Value.CentreId);
```

The excerpt stops at line 90; use the original file for the remaining 2 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
