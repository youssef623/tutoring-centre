# `tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs`

## Purpose

Declares `AssignGroupTeacherHandlerTests`, `Sut`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 148 lines and 6,290 bytes. Source hash: `2c04b45917d747a7b39995b72c55f752073cb1223266736927d8e0723ce9f0bd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L8): `public sealed class AssignGroupTeacherHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L13): `public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L26): `public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFoundAndKeepsTeacher()`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L39): `public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchivedAndKeepsTeacher()`
- [Line 53](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L53): `public async Task HandleAsync_ActiveTeacher_Reassigns()`
- [Line 65](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L65): `public async Task HandleAsync_SameTeacher_SucceedsWithNoChange()`
- [Line 77](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L77): `public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()`
- [Line 91](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L91): `public async Task HandleAsync_ActiveTeacher_MovesFutureScheduledSessionsAndLeavesPastAndCancelledOnes()`
- [Line 126](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L126): `private static Sut CreateSut()`
- [Line 142](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L142): `private sealed record Sut(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class AssignGroupTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(Guid.CreateVersion7(), sut.OtherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFoundAndKeepsTeacher()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchivedAndKeepsTeacher()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.ArchivedTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.teacher_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_Reassigns()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.OtherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(sut.OtherTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_SameTeacher_SucceedsWithNoChange()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.CurrentTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()
    {
        var sut = CreateSut();
        sut.Group.Archive();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.OtherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
```

The excerpt stops at line 90; use the original file for the remaining 58 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
