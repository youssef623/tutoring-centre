# `tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs`

## Purpose

Declares `ArchiveRestoreStudentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 128 lines and 5,680 bytes. Source hash: `eac3a589ed12baaf8f5d896a6809aa7a71e8ef8d8879c49ed97147bd1cf621ac`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;`
- Line 2: `using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L9): `public sealed class ArchiveRestoreStudentHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L14): `public async Task Archive_ActiveStudent_Succeeds()`
- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L28): `public async Task Archive_AlreadyArchived_ReturnsAlreadyArchived()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L43): `public async Task Archive_UnknownStudentId_ReturnsStudentNotFound()`
- [Line 54](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L54): `public async Task Archive_StudentWithAnActiveEnrollment_ReturnsHasActiveEnrollmentsAndStaysActive()`
- [Line 72](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L72): `public async Task Archive_StudentWhoseEnrollmentsHaveAllEnded_Succeeds()`
- [Line 90](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L90): `public async Task Restore_ArchivedStudent_Succeeds()`
- [Line 105](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L105): `public async Task Restore_AlreadyActive_ReturnsNotArchived()`
- [Line 119](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L119): `public async Task Restore_UnknownStudentId_ReturnsStudentNotFound()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveRestoreStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task Archive_ActiveStudent_Succeeds()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new ArchiveStudentHandler(students, new FakeEnrollmentRepository());

        var result = await handler.HandleAsync(new ArchiveStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StudentStatus.Archived, student.Status);
    }

    [Fact]
    public async Task Archive_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        student.Archive();
        students.Existing.Add(student);
        var handler = new ArchiveStudentHandler(students, new FakeEnrollmentRepository());

        var result = await handler.HandleAsync(new ArchiveStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.already_archived", result.Error!.Code);
    }

    [Fact]
    public async Task Archive_UnknownStudentId_ReturnsStudentNotFound()
    {
        var handler = new ArchiveStudentHandler(new FakeStudentRepository(), new FakeEnrollmentRepository());

        var result = await handler.HandleAsync(new ArchiveStudentCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task Archive_StudentWithAnActiveEnrollment_ReturnsHasActiveEnrollmentsAndStaysActive()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var enrollments = new FakeEnrollmentRepository();
        enrollments.Existing.Add(Enrollment.Start(NileCentreId, student.Id, Guid.CreateVersion7(), DateTimeOffset.UtcNow).Value);
        var handler = new ArchiveStudentHandler(students, enrollments);

        var result = await handler.HandleAsync(new ArchiveStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.has_active_enrollments", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(StudentStatus.Active, student.Status);
    }

    [Fact]
    public async Task Archive_StudentWhoseEnrollmentsHaveAllEnded_Succeeds()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var enrollments = new FakeEnrollmentRepository();
        var ended = Enrollment.Start(NileCentreId, student.Id, Guid.CreateVersion7(), DateTimeOffset.UtcNow.AddDays(-1)).Value;
        ended.End(DateTimeOffset.UtcNow, EnrollmentEndReason.Withdrawn);
        enrollments.Existing.Add(ended);
        var handler = new ArchiveStudentHandler(students, enrollments);

        var result = await handler.HandleAsync(new ArchiveStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StudentStatus.Archived, student.Status);
    }

    [Fact]
    public async Task Restore_ArchivedStudent_Succeeds()
```

The excerpt stops at line 90; use the original file for the remaining 38 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
