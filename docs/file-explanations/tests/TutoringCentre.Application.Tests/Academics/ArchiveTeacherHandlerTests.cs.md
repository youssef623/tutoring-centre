# `tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs`

## Purpose

Declares `ArchiveTeacherHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 93 lines and 3,939 bytes. Source hash: `b3322dae16e5f3c441098174410b3727f8c245ffc347b32ec8e48046240ff736`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L8): `public sealed class ArchiveTeacherHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L26): `public async Task HandleAsync_ActiveTeacher_ArchivesAndKeepsLink()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L43): `public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L59): `public async Task HandleAsync_TeacherWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()`
- [Line 77](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L77): `public async Task HandleAsync_TeacherWithOnlyArchivedGroups_Archives()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeTeacherRepository();
        var handler = new ArchiveTeacherHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_ArchivesAndKeepsLink()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var userId = Guid.CreateVersion7();
        teacher.LinkUser(userId);
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new ArchiveTeacherHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherStatus.Archived, teacher.Status);
        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.Archive();
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new ArchiveTeacherHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_TeacherWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var groups = new FakeGroupRepository();
        groups.Existing.Add(Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), teacher.Id, 20).Value);
        var handler = new ArchiveTeacherHandler(repository, groups);

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.has_active_groups", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(TeacherStatus.Active, teacher.Status);
    }

    [Fact]
    public async Task HandleAsync_TeacherWithOnlyArchivedGroups_Archives()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var archivedGroup = Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), teacher.Id, 20).Value;
        archivedGroup.Archive();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(archivedGroup);
        var handler = new ArchiveTeacherHandler(repository, groups);

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
```

The excerpt stops at line 90; use the original file for the remaining 3 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
