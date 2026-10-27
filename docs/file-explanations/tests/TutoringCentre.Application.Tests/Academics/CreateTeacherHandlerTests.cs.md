# `tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs`

## Purpose

Declares `CreateTeacherHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 76 lines and 3,338 bytes. Source hash: `cfdc519259c655e5bc50f02eb167e688480143dc9eff4084101119341bb47dfb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L9): `public sealed class CreateTeacherHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L14): `public async Task HandleAsync_Valid_AddsTeacherWithActorsCentreAndReturnsId()`
- [Line 31](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L31): `public async Task HandleAsync_TwoTeachersSharingAName_BothSucceed()`
- [Line 47](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L47): `public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()`
- [Line 62](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L62): `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndAddsNothing()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_AddsTeacherWithActorsCentreAndReturnsId()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", "+201012345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal(NileCentreId, repository.Added[0].CentreId);
        Assert.Equal("Amina Hassan", repository.Added[0].FullName);
        Assert.Equal(repository.Added[0].Id, result.Value.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_TwoTeachersSharingAName_BothSucceed()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var first = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", null), CancellationToken.None);
        var second = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", null), CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(2, repository.Added.Count);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateTeacherCommand("   ", null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.name_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", "not-a-phone"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.Validation, result.Error!.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
