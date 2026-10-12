using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;

internal sealed class ArchiveStudentValidator : AbstractValidator<ArchiveStudentCommand>
{
    public ArchiveStudentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
    }
}
