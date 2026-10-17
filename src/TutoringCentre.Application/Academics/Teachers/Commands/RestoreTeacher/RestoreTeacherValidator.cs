using FluentValidation;

namespace TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;

internal sealed class RestoreTeacherValidator : AbstractValidator<RestoreTeacherCommand>
{
    public RestoreTeacherValidator()
    {
        RuleFor(command => command.TeacherId).NotEmpty();
    }
}
