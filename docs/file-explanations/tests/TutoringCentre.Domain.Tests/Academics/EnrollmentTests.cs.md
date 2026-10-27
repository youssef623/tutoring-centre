# `tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs`

## Purpose

Declares `EnrollmentTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 104 lines and 4,198 bytes. Source hash: `3eade76c905800a4eca42c0e1c74f153fa41957f5463f7661dd46db296fbbc6b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L6): `public sealed class EnrollmentTests`
- [Line 14](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L14): `public void Start_WithValidInput_IsActiveWithNoEndFields()`
- [Line 31](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L31): `public void Start_WithEmptyCentre_ReturnsCentreRequired() =>`
- [Line 35](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L35): `public void Start_WithEmptyStudent_ReturnsStudentRequired() =>`
- [Line 39](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L39): `public void Start_WithEmptyGroup_ReturnsGroupRequired() =>`
- [Line 45](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L45): `public void End_SetsEndedWithTheGivenTimeAndReason(EnrollmentEndReason reason)`
- [Line 59](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L59): `public void End_AtTheSameInstantItStarted_Succeeds()`
- [Line 67](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L67): `public void End_Twice_ReturnsAlreadyEndedAndKeepsTheFirstEnd()`
- [Line 83](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L83): `public void End_BeforeItStarted_ReturnsEndBeforeStartAndStaysActive()`
- [Line 97](../../../../../tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs#L97): `private static void AssertValidationFailure(Result<Enrollment> result, string expectedCode)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class EnrollmentTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid StudentId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly DateTimeOffset EnrolledAt = new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Start_WithValidInput_IsActiveWithNoEndFields()
    {
        var result = Enrollment.Start(CentreId, StudentId, GroupId, EnrolledAt);

        Assert.True(result.IsSuccess);
        var enrollment = result.Value;
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Equal(CentreId, enrollment.CentreId);
        Assert.Equal(StudentId, enrollment.StudentId);
        Assert.Equal(GroupId, enrollment.GroupId);
        Assert.Equal(EnrolledAt, enrollment.EnrolledAt);
        Assert.Null(enrollment.EndedAt);
        Assert.Null(enrollment.EndReason);
        Assert.NotEqual(Guid.Empty, enrollment.Id);
    }

    [Fact]
    public void Start_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Enrollment.Start(Guid.Empty, StudentId, GroupId, EnrolledAt), "enrollment.centre_required");

    [Fact]
    public void Start_WithEmptyStudent_ReturnsStudentRequired() =>
        AssertValidationFailure(Enrollment.Start(CentreId, Guid.Empty, GroupId, EnrolledAt), "enrollment.student_required");

    [Fact]
    public void Start_WithEmptyGroup_ReturnsGroupRequired() =>
        AssertValidationFailure(Enrollment.Start(CentreId, StudentId, Guid.Empty, EnrolledAt), "enrollment.group_required");

    [Theory]
    [InlineData(EnrollmentEndReason.Withdrawn)]
    [InlineData(EnrollmentEndReason.Transferred)]
    public void End_SetsEndedWithTheGivenTimeAndReason(EnrollmentEndReason reason)
    {
        var enrollment = Enrollment.Start(CentreId, StudentId, GroupId, EnrolledAt).Value;
        var endedAt = EnrolledAt.AddDays(30);

        var result = enrollment.End(endedAt, reason);

        Assert.True(result.IsSuccess);
        Assert.Equal(EnrollmentStatus.Ended, enrollment.Status);
        Assert.Equal(endedAt, enrollment.EndedAt);
        Assert.Equal(reason, enrollment.EndReason);
    }

    [Fact]
    public void End_AtTheSameInstantItStarted_Succeeds()
    {
        var enrollment = Enrollment.Start(CentreId, StudentId, GroupId, EnrolledAt).Value;

        Assert.True(enrollment.End(EnrolledAt, EnrollmentEndReason.Withdrawn).IsSuccess);
    }

    [Fact]
    public void End_Twice_ReturnsAlreadyEndedAndKeepsTheFirstEnd()
    {
        var enrollment = Enrollment.Start(CentreId, StudentId, GroupId, EnrolledAt).Value;
        var firstEnd = EnrolledAt.AddDays(10);
        Assert.True(enrollment.End(firstEnd, EnrollmentEndReason.Withdrawn).IsSuccess);

        var result = enrollment.End(EnrolledAt.AddDays(20), EnrollmentEndReason.Transferred);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_ended", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(firstEnd, enrollment.EndedAt);
        Assert.Equal(EnrollmentEndReason.Withdrawn, enrollment.EndReason);
    }

    [Fact]
    public void End_BeforeItStarted_ReturnsEndBeforeStartAndStaysActive()
    {
        var enrollment = Enrollment.Start(CentreId, StudentId, GroupId, EnrolledAt).Value;

        var result = enrollment.End(EnrolledAt.AddTicks(-1), EnrollmentEndReason.Withdrawn);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.end_before_start", result.Error!.Code);
```

The excerpt stops at line 90; use the original file for the remaining 14 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
