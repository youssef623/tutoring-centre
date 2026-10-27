# `tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs`

## Purpose

Declares `ArchiveGroupHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 132 lines and 5,266 bytes. Source hash: `bda3b9573a1dcab3ab2e1c9b1421e84b92589146e45cb5571b14196418c8229b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L8): `public sealed class ArchiveGroupHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L25): `public async Task HandleAsync_ActiveGroup_Archives()`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L39): `public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()`
- [Line 55](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L55): `public async Task HandleAsync_GroupWithActiveEnrollments_ReturnsHasActiveEnrollmentsAndStaysActive()`
- [Line 72](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L72): `public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndStaysActive()`
- [Line 88](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L88): `public async Task HandleAsync_GroupWithARuleStillInEffect_ReturnsHasActiveScheduleAndStaysActive()`
- [Line 106](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L106): `public async Task HandleAsync_GroupWhoseOnlyRuleEndedBeforeToday_Archives()`
- [Line 124](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L124): `private static ArchiveGroupHandler NewHandler(FakeGroupRepository groups) =>`
- [Line 127](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L127): `private static ArchiveGroupHandler NewHandler(FakeGroupRepository groups, FakeScheduleRuleRepository rules) =>`
- [Line 130](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L130): `private static Group NewGroup() =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = NewHandler(new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveGroupCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveGroup_Archives()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(GroupStatus.Archived, group.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var group = NewGroup();
        group.Archive();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_GroupWithActiveEnrollments_ReturnsHasActiveEnrollmentsAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.ActiveEnrollmentCounts[group.Id] = 1;
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.has_active_enrollments", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.Versions[group.Id] = 3;
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 2), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task HandleAsync_GroupWithARuleStillInEffect_ReturnsHasActiveScheduleAndStaysActive()
    {
        var group = NewGroup();
```

The excerpt stops at line 90; use the original file for the remaining 42 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
