# `tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Branching of the transfer handler against fakes. Every failure asserts both effects are absent together — the old enrollment still active and untouched, and no new enrollment added — not each one on its own.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 165 lines and 6,596 bytes. Source hash: `a0043ff3a1bfae8b02183d824bec16995c099971eb12fe83c5070fa08d2308a6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L14): `public sealed class TransferEnrollmentHandlerTests`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L19): `public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L29): `public async Task HandleAsync_EndedEnrollment_ReturnsAlreadyEnded()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L43): `public async Task HandleAsync_SameGroup_ReturnsSameGroup()`
- [Line 53](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L53): `public async Task HandleAsync_UnknownTarget_ReturnsGroupNotFound()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L63): `public async Task HandleAsync_TargetOfAnotherSubject_ReturnsSubjectMismatch()`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L75): `public async Task HandleAsync_StudentAlreadyInTarget_ReturnsAlreadyEnrolled()`
- [Line 86](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L86): `public async Task HandleAsync_TargetFull_ReturnsGroupFullAndChangesNothing()`
- [Line 97](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L97): `public async Task HandleAsync_Success_EndsTheOldAsTransferredAndAddsANewActiveEnrollmentInTheTarget()`
- [Line 117](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L117): `private sealed class Sut`
- [Line 119](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L119): `public Sut()`
- [Line 147](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L147): `public Task<Result<TransferEnrollmentResult>> TransferAsync(Guid enrollmentId, Guid targetGroupId) =>`
- [Line 151](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L151): `public void AssertNothingChanged(Result<TransferEnrollmentResult> result, string code, ErrorKind kind)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Branching of the transfer handler against fakes. Every failure asserts both effects are absent together —
/// the old enrollment still active and untouched, and no new enrollment added — not each one on its own.
/// </summary>
public sealed class TransferEnrollmentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(Guid.CreateVersion7(), sut.Target.Id);

        sut.AssertNothingChanged(result, "enrollment.not_found", ErrorKind.NotFound);
    }

    [Fact]
    public async Task HandleAsync_EndedEnrollment_ReturnsAlreadyEnded()
    {
        var sut = new Sut();
        sut.Current.End(sut.Clock.UtcNow.AddHours(-1), EnrollmentEndReason.Withdrawn);

        var result = await sut.TransferAsync(sut.Current.Id, sut.Target.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_ended", result.Error!.Code);
        Assert.Equal(EnrollmentEndReason.Withdrawn, sut.Current.EndReason);
        Assert.Empty(sut.Enrollments.Added);
    }

    [Fact]
    public async Task HandleAsync_SameGroup_ReturnsSameGroup()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(sut.Current.Id, sut.Source.Id);

        sut.AssertNothingChanged(result, "enrollment.same_group", ErrorKind.Rule);
    }

    [Fact]
    public async Task HandleAsync_UnknownTarget_ReturnsGroupNotFound()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(sut.Current.Id, Guid.CreateVersion7());

        sut.AssertNothingChanged(result, "group.not_found", ErrorKind.NotFound);
    }

    [Fact]
    public async Task HandleAsync_TargetOfAnotherSubject_ReturnsSubjectMismatch()
    {
        var sut = new Sut();
        var otherSubject = Group.Create(NileCentreId, "Physics 10", Guid.CreateVersion7(), sut.Source.TeacherId, 20).Value;
        sut.Groups.Existing.Add(otherSubject);

        var result = await sut.TransferAsync(sut.Current.Id, otherSubject.Id);

        sut.AssertNothingChanged(result, "enrollment.subject_mismatch", ErrorKind.Rule);
    }

    [Fact]
    public async Task HandleAsync_StudentAlreadyInTarget_ReturnsAlreadyEnrolled()
    {
        var sut = new Sut();
        sut.Enrollments.Existing.Add(Enrollment.Start(NileCentreId, sut.Current.StudentId, sut.Target.Id, sut.Clock.UtcNow.AddDays(-1)).Value);

        var result = await sut.TransferAsync(sut.Current.Id, sut.Target.Id);

        sut.AssertNothingChanged(result, "enrollment.already_enrolled", ErrorKind.Conflict);
    }

    [Fact]
    public async Task HandleAsync_TargetFull_ReturnsGroupFullAndChangesNothing()
    {
        var sut = new Sut();
        sut.Groups.ActiveEnrollmentCounts[sut.Target.Id] = sut.Target.Capacity;

```

The excerpt stops at line 90; use the original file for the remaining 75 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
