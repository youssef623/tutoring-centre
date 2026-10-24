using FluentValidation;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;

internal sealed class EnrollStudentValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.StudentId).NotEmpty();
    }
}
