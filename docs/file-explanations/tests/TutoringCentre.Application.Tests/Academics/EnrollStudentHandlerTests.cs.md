# `tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Branching of the enroll handler against fakes: the order of checks and the code each one answers. The fake group repository reports a configurable active count and takes no lock; the lock itself is proven against a real database (LastSeatConcurrencyTests).

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 145 lines and 5,626 bytes. Source hash: `b103c296059aae3280f7e7b395eba4f594463796a31f9c5c08d2746efdedaff5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L15): `public sealed class EnrollStudentHandlerTests`
- [Line 20](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L20): `public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L30): `public async Task HandleAsync_UnknownGroupAndUnknownStudent_ReturnsGroupNotFoundBeforeAnythingAboutTheStudent()`
- [Line 40](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L40): `public async Task HandleAsync_UnknownStudent_ReturnsStudentNotFound()`
- [Line 50](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L50): `public async Task HandleAsync_ArchivedStudent_ReturnsStudentArchived()`
- [Line 61](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L61): `public async Task HandleAsync_AlreadyEnrolled_ReturnsAlreadyEnrolled()`
- [Line 72](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L72): `public async Task HandleAsync_CountEqualToCapacity_ReturnsGroupFull()`
- [Line 83](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L83): `public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchived()`
- [Line 94](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L94): `public async Task HandleAsync_LastSeat_AddsOneActiveEnrollmentInTheActorsCentreAtTheClocksTime()`
- [Line 111](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L111): `private static void AssertFailure(Result<EnrollStudentResult> result, string code, ErrorKind kind, Sut sut)`
- [Line 119](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L119): `private sealed class Sut`
- [Line 121](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L121): `public Sut()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Branching of the enroll handler against fakes: the order of checks and the code each one answers. The fake
/// group repository reports a configurable active count and takes no lock; the lock itself is proven against a
/// real database (LastSeatConcurrencyTests).
/// </summary>
public sealed class EnrollStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()
    {
        var sut = new Sut();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(Guid.CreateVersion7(), sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "group.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_UnknownGroupAndUnknownStudent_ReturnsGroupNotFoundBeforeAnythingAboutTheStudent()
    {
        var sut = new Sut();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(Guid.CreateVersion7(), Guid.CreateVersion7()), CancellationToken.None);

        AssertFailure(result, "group.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_UnknownStudent_ReturnsStudentNotFound()
    {
        var sut = new Sut();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, Guid.CreateVersion7()), CancellationToken.None);

        AssertFailure(result, "student.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_ArchivedStudent_ReturnsStudentArchived()
    {
        var sut = new Sut();
        sut.Student.Archive();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "enrollment.student_archived", ErrorKind.Rule, sut);
    }

    [Fact]
    public async Task HandleAsync_AlreadyEnrolled_ReturnsAlreadyEnrolled()
    {
        var sut = new Sut();
        sut.Enrollments.Existing.Add(Enrollment.Start(NileCentreId, sut.Student.Id, sut.Group.Id, sut.Clock.UtcNow.AddDays(-1)).Value);

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "enrollment.already_enrolled", ErrorKind.Conflict, sut);
    }

    [Fact]
    public async Task HandleAsync_CountEqualToCapacity_ReturnsGroupFull()
    {
        var sut = new Sut();
        sut.Groups.ActiveEnrollmentCounts[sut.Group.Id] = sut.Group.Capacity;

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "group.full", ErrorKind.Conflict, sut);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchived()
    {
        var sut = new Sut();
        sut.Group.Archive();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "group.archived", ErrorKind.Rule, sut);
```

The excerpt stops at line 90; use the original file for the remaining 55 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
