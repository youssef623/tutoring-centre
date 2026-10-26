using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

/// <summary>Shape checks only. The name's cleaning and real validity live in Subject.Rename — the single source of truth.</summary>
internal sealed class RenameSubjectValidator : AbstractValidator<RenameSubjectCommand>
{
    public RenameSubjectValidator()
    {
        RuleFor(command => command.SubjectId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Subject.NameMaxLength);
    }
}
