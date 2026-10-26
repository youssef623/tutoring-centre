using FluentValidation;

namespace TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;

internal sealed class ArchiveSubjectValidator : AbstractValidator<ArchiveSubjectCommand>
{
    public ArchiveSubjectValidator()
    {
        RuleFor(command => command.SubjectId).NotEmpty();
    }
}
