# `tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs`

## Purpose

Declares `UpdateStudentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 83 lines and 3,581 bytes. Source hash: `1fe13630e4f0549a9e6ba3ba0f53ad41a45e54246e0a21a3d98d9ff707c3e915`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L8): `public sealed class UpdateStudentHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L13): `public async Task HandleAsync_Valid_ReplacesAllThreeFields()`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L30): `public async Task HandleAsync_OmittedPhoneAndSchoolYear_ClearsBoth()`
- [Line 45](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L45): `public async Task HandleAsync_UnknownStudentId_ReturnsStudentNotFound()`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L56): `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()`
- [Line 70](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L70): `public async Task HandleAsync_ArchivedStudent_ReturnsArchivedReadOnly()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_ReplacesAllThreeFields()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, "Grade 9", Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(
            new UpdateStudentCommand(student.Id, "Youssef Omar Hassan", "01012345678", "Grade 10", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Youssef Omar Hassan", student.FullName);
        Assert.Equal("+201012345678", student.Phone!.Value);
        Assert.Equal("Grade 10", student.SchoolYear);
    }

    [Fact]
    public async Task HandleAsync_OmittedPhoneAndSchoolYear_ClearsBoth()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", PhoneNumber.Create("01012345678").Value, "Grade 9", Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(new UpdateStudentCommand(student.Id, "Youssef Omar", null, null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(student.Phone);
        Assert.Null(student.SchoolYear);
    }

    [Fact]
    public async Task HandleAsync_UnknownStudentId_ReturnsStudentNotFound()
    {
        var handler = new UpdateStudentHandler(new FakeStudentRepository());

        var result = await handler.HandleAsync(new UpdateStudentCommand(Guid.CreateVersion7(), "Youssef Omar", null, null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(new UpdateStudentCommand(student.Id, "Youssef Omar", "12345", null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("phone", result.Error!.Fields!.Keys);
    }

    [Fact]
    public async Task HandleAsync_ArchivedStudent_ReturnsArchivedReadOnly()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        student.Archive();
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(new UpdateStudentCommand(student.Id, "Youssef Omar Hassan", null, null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.archived_read_only", result.Error!.Code);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
