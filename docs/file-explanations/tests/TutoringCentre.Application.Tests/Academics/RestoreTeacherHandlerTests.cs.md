# `tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs`

## Purpose

Declares `RestoreTeacherHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 54 lines and 2,037 bytes. Source hash: `ec90f322151ebfc3d062b26bea3c7f923166475b942b9976af6f3e2f5264f660`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L8): `public sealed class RestoreTeacherHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L26): `public async Task HandleAsync_ArchivedTeacher_Restores()`
- [Line 41](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L41): `public async Task HandleAsync_NotArchived_ReturnsNotArchived()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeTeacherRepository();
        var handler = new RestoreTeacherHandler(repository);

        var result = await handler.HandleAsync(new RestoreTeacherCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_Restores()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.Archive();
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new RestoreTeacherHandler(repository);

        var result = await handler.HandleAsync(new RestoreTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherStatus.Active, teacher.Status);
    }

    [Fact]
    public async Task HandleAsync_NotArchived_ReturnsNotArchived()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new RestoreTeacherHandler(repository);

        var result = await handler.HandleAsync(new RestoreTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
