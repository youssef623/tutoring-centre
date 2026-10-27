# `tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs`

## Purpose

Declares `UpdateTeacherHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 2,889 bytes. Source hash: `0519e0db9b87b13378776d534f248744ef3a0bd5ff595f0f4ebcd40e2eb68560`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L8): `public sealed class UpdateTeacherHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L13): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L27): `public async Task HandleAsync_Valid_UpdatesNameAndPhone()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L43): `public async Task HandleAsync_ArchivedTeacher_ReturnsArchivedReadOnly()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L59): `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndLeavesTeacherUnchanged()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeTeacherRepository();
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(Guid.CreateVersion7(), "Amina Hassan", null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_Valid_UpdatesNameAndPhone()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(teacher.Id, "Amina Youssef", "+201012345678", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Amina Youssef", teacher.FullName);
        Assert.Equal("+201012345678", teacher.Phone!.Value);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_ReturnsArchivedReadOnly()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.Archive();
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(teacher.Id, "Amina Youssef", null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.archived_read_only", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndLeavesTeacherUnchanged()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(teacher.Id, "Amina Hassan", "not-a-phone", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.Validation, result.Error!.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Null(teacher.Phone);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
