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
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Null(enrollment.EndedAt);
        Assert.Null(enrollment.EndReason);
    }

    private static void AssertValidationFailure(Result<Enrollment> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
