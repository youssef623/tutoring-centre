using FluentValidation;

namespace TutoringCentre.Application.Attendance.Commands.CorrectAttendance;

internal sealed class CorrectAttendanceValidator : AbstractValidator<CorrectAttendanceCommand>
{
    public CorrectAttendanceValidator()
    {
        RuleFor(command => command.RecordId).NotEmpty();
        RuleFor(command => command.Status).IsInEnum();
        RuleFor(command => command.Reason).NotNull().Length(1, 400);
    }
}
