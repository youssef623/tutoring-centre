using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;

internal sealed class RestoreStudentValidator : AbstractValidator<RestoreStudentCommand>
{
    public RestoreStudentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
    }
}
