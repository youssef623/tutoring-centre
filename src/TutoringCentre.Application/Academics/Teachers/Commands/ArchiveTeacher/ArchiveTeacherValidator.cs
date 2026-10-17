using FluentValidation;

namespace TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

internal sealed class ArchiveTeacherValidator : AbstractValidator<ArchiveTeacherCommand>
{
    public ArchiveTeacherValidator()
    {
        RuleFor(command => command.TeacherId).NotEmpty();
    }
}
