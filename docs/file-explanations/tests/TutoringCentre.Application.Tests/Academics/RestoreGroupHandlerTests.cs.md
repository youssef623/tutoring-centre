# `tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs`

## Purpose

Declares `RestoreGroupHandlerTests`, `Sut`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 95 lines and 3,397 bytes. Source hash: `5585a7947c9d6be0a67a34d1c2b7282a4992d0fd72c578756567a2aa58ee17af`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L8): `public sealed class RestoreGroupHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L25): `public async Task HandleAsync_SubjectArchived_ReturnsSubjectArchivedAndStaysArchived()`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L39): `public async Task HandleAsync_TeacherArchived_ReturnsTeacherArchivedAndStaysArchived()`
- [Line 53](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L53): `public async Task HandleAsync_BothReferencesActive_Restores()`
- [Line 64](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L64): `public async Task HandleAsync_NotArchived_ReturnsNotArchived()`
- [Line 76](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L76): `private static Sut CreateSut()`
- [Line 94](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L94): `private sealed record Sut(RestoreGroupHandler Handler, Group Group, Subject Subject, Teacher Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SubjectArchived_ReturnsSubjectArchivedAndStaysArchived()
    {
        var sut = CreateSut();
        sut.Subject.Archive();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.subject_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Archived, sut.Group.Status);
    }

    [Fact]
    public async Task HandleAsync_TeacherArchived_ReturnsTeacherArchivedAndStaysArchived()
    {
        var sut = CreateSut();
        sut.Teacher.Archive();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.teacher_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Archived, sut.Group.Status);
    }

    [Fact]
    public async Task HandleAsync_BothReferencesActive_Restores()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(GroupStatus.Active, sut.Group.Status);
    }

    [Fact]
    public async Task HandleAsync_NotArchived_ReturnsNotArchived()
    {
        var sut = CreateSut();
        sut.Group.Restore();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    private static Sut CreateSut()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var subjects = new FakeSubjectRepository();
        subjects.Existing.Add(subject);

        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var teachers = new FakeTeacherRepository();
        teachers.Existing.Add(teacher);

        var group = Group.Create(NileCentreId, "Grade 10 A", subject.Id, teacher.Id, 20).Value;
        group.Archive();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(group);

```

The excerpt stops at line 90; use the original file for the remaining 5 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
