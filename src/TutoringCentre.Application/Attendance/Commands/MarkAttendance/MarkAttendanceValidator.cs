using FluentValidation;

namespace TutoringCentre.Application.Attendance.Commands.MarkAttendance;

internal sealed class MarkAttendanceValidator : AbstractValidator<MarkAttendanceCommand>
{
    public MarkAttendanceValidator()
    {
        RuleFor(command => command.SessionId).NotEmpty();
        RuleFor(command => command.Marks).NotNull()
            .Must(marks => marks is { Count: >= 1 and <= 200 }).WithMessage("Provide between 1 and 200 marks.");
        RuleFor(command => command).Custom((command, context) =>
        {
            if (command.Marks is null)
            {
                return;
            }

            var seen = new HashSet<Guid>();
            for (var index = 0; index < command.Marks.Count; index++)
            {
                var mark = command.Marks[index];
                if (mark is null)
                {
                    context.AddFailure($"marks[{index}]", "A mark is required.");
                    continue;
                }

                if (mark.StudentId == Guid.Empty || !seen.Add(mark.StudentId))
                {
                    context.AddFailure($"marks[{index}].studentId", "Provide a non-empty, distinct student ID.");
                }

                if (!Enum.IsDefined(mark.Status))
                {
                    context.AddFailure($"marks[{index}].status", "Choose a defined attendance status.");
                }
            }
        });
    }
}
