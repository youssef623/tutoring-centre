# `tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs`

## Purpose

Declares `RenameSubjectHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 3,333 bytes. Source hash: `348de97dff13698724a542b7155e714cf2cdff36d397da767975f4b0371d232f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L8): `public sealed class RenameSubjectHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L27): `public async Task HandleAsync_NewNameAlreadyTakenByAnotherSubject_ReturnsNameTaken()`
- [Line 44](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L44): `public async Task HandleAsync_SameName_SucceedsWithNoChange()`
- [Line 58](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L58): `public async Task HandleAsync_ArchivedSubject_ReturnsArchivedReadOnly()`
- [Line 73](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L73): `public async Task HandleAsync_ValidNewName_Succeeds()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RenameSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeSubjectRepository();
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(
            new RenameSubjectCommand(Guid.CreateVersion7(), "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NewNameAlreadyTakenByAnotherSubject_ReturnsNameTaken()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var other = Subject.Create(NileCentreId, "Chemistry").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        repository.Existing.Add(other);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Chemistry", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SameName_SucceedsWithNoChange()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Mathematics", subject.Name);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubject_ReturnsArchivedReadOnly()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        subject.Archive();
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.archived_read_only", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ValidNewName_Succeeds()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Applied Mathematics", subject.Name);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
