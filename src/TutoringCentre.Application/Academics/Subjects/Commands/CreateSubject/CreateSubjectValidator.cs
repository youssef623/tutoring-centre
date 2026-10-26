using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

/// <summary>Shape checks only. The name's cleaning and real validity live in Subject.Create — the single source of truth.</summary>
internal sealed class CreateSubjectValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Subject.NameMaxLength);
    }
}
