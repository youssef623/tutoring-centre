# `tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs`

## Purpose

Declares `ChangeGroupCapacityHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 118 lines and 4,570 bytes. Source hash: `2afffc14eb91bd5806f21162dbd22d24efbae06f215f0e7b1c2bb6137b331674`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L8): `public sealed class ChangeGroupCapacityHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L25): `public async Task HandleAsync_InRange_ChangesCapacity()`
- [Line 38](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L38): `public async Task HandleAsync_OutOfRange_ReturnsCapacityInvalidAndKeepsCapacity(int capacity)`
- [Line 51](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L51): `public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()`
- [Line 64](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L64): `public async Task HandleAsync_BelowTheActiveEnrollmentCount_ReturnsCapacityBelowEnrolledAndKeepsCapacity()`
- [Line 78](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L78): `public async Task HandleAsync_EqualToTheActiveEnrollmentCount_ChangesCapacity()`
- [Line 90](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L90): `public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndKeepsCapacity()`
- [Line 103](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L103): `private static (ChangeGroupCapacityHandler Handler, Group Group) CreateSut()`
- [Line 111](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L111): `private static (ChangeGroupCapacityHandler Handler, Group Group, FakeGroupRepository Repository) CreateSutWithRepository()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ChangeGroupCapacityHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new ChangeGroupCapacityHandler(new FakeGroupRepository());

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(Guid.CreateVersion7(), 30, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_InRange_ChangesCapacity()
    {
        var (handler, group) = CreateSut();

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, 200, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(200, group.Capacity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task HandleAsync_OutOfRange_ReturnsCapacityInvalidAndKeepsCapacity(int capacity)
    {
        var (handler, group) = CreateSut();

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, capacity, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.capacity_invalid", result.Error!.Code);
        Assert.Contains("capacity", result.Error.Fields!.Keys);
        Assert.Equal(20, group.Capacity);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()
    {
        var (handler, group) = CreateSut();
        group.Archive();

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, 30, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(20, group.Capacity);
    }

    [Fact]
    public async Task HandleAsync_BelowTheActiveEnrollmentCount_ReturnsCapacityBelowEnrolledAndKeepsCapacity()
    {
        var (handler, group, repository) = CreateSutWithRepository();
        repository.ActiveEnrollmentCounts[group.Id] = 15;

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, 14, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.capacity_below_enrolled", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(20, group.Capacity);
    }

    [Fact]
    public async Task HandleAsync_EqualToTheActiveEnrollmentCount_ChangesCapacity()
    {
        var (handler, group, repository) = CreateSutWithRepository();
        repository.ActiveEnrollmentCounts[group.Id] = 15;

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, 15, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(15, group.Capacity);
    }

    [Fact]
    public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndKeepsCapacity()
```

The excerpt stops at line 90; use the original file for the remaining 28 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
