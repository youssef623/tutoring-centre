# `tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`

## Purpose

Declares `DispatcherTests`, `Fixture`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 390 lines and 16,038 bytes. Source hash: `7332787d3d5b837d55075a7e2da529f9950dc8a8e731a38c4c4e1219dc1cfe83`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Application.Tests.Fakes;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L12): `public sealed class DispatcherTests`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L17): `public async Task SendAsync_ValidCommandHandlerSucceeds_BeginsHandlesSavesAndCommits()`
- [Line 31](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L31): `public async Task SendAsync_InvalidCommand_DoesNotBeginOrCallHandler()`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L46): `public async Task SendAsync_HandlerReturnsFailure_RollsBackWithoutSavingOrCommitting()`
- [Line 58](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L58): `public async Task SendAsync_HandlerThrows_RollsBackAndRethrows()`
- [Line 69](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L69): `public async Task SendAsync_SaveChangesThrows_RollsBackAndDoesNotCommit()`
- [Line 81](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L81): `public async Task SendAsync_SaveFails_RollsBackWithoutCommittingAndReturnsTheSaveError()`
- [Line 96](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L96): `public async Task QueryAsync_ValidQuery_BeginsReadOnlyHandlesAndCommitsWithoutSaving()`
- [Line 108](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L108): `public async Task QueryAsync_InvalidQuery_DoesNotBeginOrCallHandler()`
- [Line 120](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L120): `public async Task QueryAsync_HandlerThrows_RollsBackAndRethrows()`
- [Line 131](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L131): `public async Task SendAsync_SeveralFieldFailures_GroupsMessagesByCamelCaseFieldName()`
- [Line 144](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L144): `public async Task SendAsync_TenantScopedCommandStaffActorWithoutCentre_ReturnsTenantNotSelected()`
- [Line 160](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L160): `public async Task QueryAsync_TenantScopedQueryStaffActorWithoutCentre_ReturnsTenantNotSelected()`
- [Line 176](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L176): `public async Task SendAsync_TenantScopedCommandAnonymousActor_ReturnsNotAuthenticated()`
- [Line 191](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L191): `public async Task QueryAsync_TenantScopedQueryAnonymousActor_ReturnsNotAuthenticated()`
- [Line 206](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L206): `public async Task SendAsync_TenantScopedCommandActorWithCentre_InvokesHandler()`
- [Line 220](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L220): `public async Task QueryAsync_TenantScopedQueryActorWithCentre_InvokesHandler()`
- [Line 234](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L234): `public async Task SendAsync_NonScopedRequestActorWithoutCentre_InvokesHandler()`
- [Line 247](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L247): `public async Task SendAsync_TenantScopedCommandInvalidAndActorWithoutCentre_ValidationWinsOverTenantStep()`
- [Line 262](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L262): `public async Task SendAsync_StaffActorWithThePermission_InvokesHandler()`
- [Line 276](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L276): `public async Task SendAsync_StaffActorWithoutThePermission_ReturnsPermissionDenied()`
- [Line 292](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L292): `public async Task QueryAsync_StaffActorWithoutThePermission_ReturnsPermissionDenied()`
- [Line 308](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L308): `public async Task SendAsync_SystemActor_IsAllowedRegardlessOfPermission()`
- [Line 322](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L322): `public async Task SendAsync_PermissionRequiringCommandActorWithoutCentre_TenantStepWins()`
- [Line 337](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L337): `public async Task QueryAsync_PermissionRequiringQueryActorWithoutCentre_TenantStepWins()`
- [Line 351](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L351): `private static Fixture CreateSut(HandlerMode mode = HandlerMode.Succeed, Actor? actor = null)`
- [Line 382](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L382): `private sealed class Fixture(Dispatcher dispatcher, FakeUnitOfWork unitOfWork, ServiceProvider provider) : IDisposable`
- [Line 388](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L388): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Cqrs;

public sealed class DispatcherTests
{
    private static readonly TestCommand ValidCommand = new("Nile", "nile");

    [Fact]
    public async Task SendAsync_ValidCommandHandlerSucceeds_BeginsHandlesSavesAndCommits()
    {
        // Arrange
        using var fixture = CreateSut();

        // Act
        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(rw)", "Handle", "Save", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_InvalidCommand_DoesNotBeginOrCallHandler()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(new TestCommand("", "nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("validation.failed", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.NotNull(result.Error.Fields);
        Assert.Contains("name", result.Error.Fields.Keys);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_HandlerReturnsFailure_RollsBackWithoutSavingOrCommitting()
    {
        using var fixture = CreateSut(HandlerMode.Fail);

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(TestErrors.HandlerFailure, result.Error);
        Assert.Equal(["Begin(rw)", "Handle", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_HandlerThrows_RollsBackAndRethrows()
    {
        using var fixture = CreateSut(HandlerMode.Throw);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None));

        Assert.Equal(["Begin(rw)", "Handle", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_SaveChangesThrows_RollsBackAndDoesNotCommit()
    {
        using var fixture = CreateSut();
        fixture.UnitOfWork.ThrowOnSave = true;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None));

        Assert.Equal(["Begin(rw)", "Handle", "Save", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_SaveFails_RollsBackWithoutCommittingAndReturnsTheSaveError()
    {
        // C14
        using var fixture = CreateSut();
        var saveError = Error.Conflict("test.save_conflict", "Someone else changed this first.");
        fixture.UnitOfWork.SaveFailure = saveError;

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        Assert.True(result.IsFailure);
```

The excerpt stops at line 90; use the original file for the remaining 300 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
