using TutoringCentre.Application.Attendance.Commands.CorrectAttendance;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class AttendanceValidatorTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(200, true)]
    [InlineData(201, false)]
    public void Marks_EnforcesBatchSize(int count, bool valid)
    {
        var command = new MarkAttendanceCommand(Guid.NewGuid(), Enumerable.Range(0, count)
            .Select(_ => new AttendanceMark(Guid.NewGuid(), AttendanceStatus.Present)).ToArray());
        Assert.Equal(valid, new MarkAttendanceValidator().Validate(command).IsValid);
    }

    [Theory]
    [InlineData("duplicate", "marks[1].studentId")]
    [InlineData("emptyId", "marks[1].studentId")]
    [InlineData("status", "marks[1].status")]
    public void InvalidMark_UsesIndexedField(string scenario, string field)
    {
        var id = Guid.NewGuid();
        AttendanceMark[] marks = [new(id, AttendanceStatus.Present), new(Guid.NewGuid(), AttendanceStatus.Late)];
        marks[1] = scenario switch
        {
            "duplicate" => marks[1] with { StudentId = id },
            "emptyId" => marks[1] with { StudentId = Guid.Empty },
            _ => marks[1] with { Status = (AttendanceStatus)99 },
        };
        var result = new MarkAttendanceValidator().Validate(new MarkAttendanceCommand(Guid.NewGuid(), marks));
        Assert.Contains(result.Errors, error => error.PropertyName == field);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(400, true)]
    [InlineData(401, false)]
    public void Correction_EnforcesRawReasonShape(int length, bool valid) =>
        Assert.Equal(valid, new CorrectAttendanceValidator().Validate(
            new CorrectAttendanceCommand(Guid.NewGuid(), AttendanceStatus.Absent, new string('x', length), 1)).IsValid);

    [Fact]
    public void Correction_WhitespacePassesRawShape_ForDomainNormalisation()
    {
        var result = new CorrectAttendanceValidator().Validate(
            new CorrectAttendanceCommand(Guid.NewGuid(), AttendanceStatus.Absent, " \t ", 1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Correction_InvalidIdAndStatus_AreRejected()
    {
        var result = new CorrectAttendanceValidator().Validate(new CorrectAttendanceCommand(Guid.Empty, (AttendanceStatus)99, "reason", 1));
        Assert.Contains(result.Errors, error => error.PropertyName == "RecordId");
        Assert.Contains(result.Errors, error => error.PropertyName == "Status");
    }
}
