# `tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs`

## Purpose

Declares `RenameGroupHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 88 lines and 3,283 bytes. Source hash: `969a60475bdd797baffcf8e3e3b3740da7d094a8b76008987b1a61abda8a347b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L8): `public sealed class RenameGroupHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L25): `public async Task HandleAsync_NewUniqueName_Renames()`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L39): `public async Task HandleAsync_SameName_SucceedsWithNoChange()`
- [Line 53](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L53): `public async Task HandleAsync_NameUsedByAnotherGroup_ReturnsNameTaken()`
- [Line 71](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L71): `public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()`
- [Line 86](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L86): `private static Group NewGroup(string name) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RenameGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new RenameGroupHandler(new FakeGroupRepository());

        var result = await handler.HandleAsync(new RenameGroupCommand(Guid.CreateVersion7(), "Grade 10 B", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NewUniqueName_Renames()
    {
        var group = NewGroup("Grade 10 A");
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new RenameGroupHandler(repository);

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "Grade 10 B", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Grade 10 B", group.Name);
    }

    [Fact]
    public async Task HandleAsync_SameName_SucceedsWithNoChange()
    {
        var group = NewGroup("Grade 10 A");
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new RenameGroupHandler(repository);

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "Grade 10 A", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Grade 10 A", group.Name);
    }

    [Fact]
    public async Task HandleAsync_NameUsedByAnotherGroup_ReturnsNameTaken()
    {
        var group = NewGroup("Grade 10 A");
        var other = NewGroup("Grade 10 B");
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.Existing.Add(other);
        var handler = new RenameGroupHandler(repository);

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "grade 10 b", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("name", result.Error.Fields!.Keys);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()
    {
        var group = NewGroup("Grade 10 A");
        group.Archive();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new RenameGroupHandler(repository);

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "Grade 10 B", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal("Grade 10 A", group.Name);
    }

    private static Group NewGroup(string name) =>
        Group.Create(NileCentreId, name, Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
