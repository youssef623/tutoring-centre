# `tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs`

## Purpose

Declares `ArchiveSubjectHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 90 lines and 3,775 bytes. Source hash: `8fd271d1bdd36e3fb3f0f017891f13c29ce68bab2dd342ea24ef1f5806110629`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L8): `public sealed class ArchiveSubjectHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L26): `public async Task HandleAsync_ActiveSubject_Archives()`
- [Line 40](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L40): `public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L56): `public async Task HandleAsync_SubjectWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()`
- [Line 74](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L74): `public async Task HandleAsync_SubjectWithOnlyArchivedGroups_Archives()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeSubjectRepository();
        var handler = new ArchiveSubjectHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveSubject_Archives()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new ArchiveSubjectHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SubjectStatus.Archived, subject.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        subject.Archive();
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new ArchiveSubjectHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SubjectWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var groups = new FakeGroupRepository();
        groups.Existing.Add(Group.Create(NileCentreId, "Grade 10 A", subject.Id, Guid.CreateVersion7(), 20).Value);
        var handler = new ArchiveSubjectHandler(repository, groups);

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.has_active_groups", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(SubjectStatus.Active, subject.Status);
    }

    [Fact]
    public async Task HandleAsync_SubjectWithOnlyArchivedGroups_Archives()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var archivedGroup = Group.Create(NileCentreId, "Grade 10 A", subject.Id, Guid.CreateVersion7(), 20).Value;
        archivedGroup.Archive();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(archivedGroup);
        var handler = new ArchiveSubjectHandler(repository, groups);

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SubjectStatus.Archived, subject.Status);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
