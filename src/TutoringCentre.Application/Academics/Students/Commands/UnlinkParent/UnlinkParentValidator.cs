using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;

internal sealed class UnlinkParentValidator : AbstractValidator<UnlinkParentCommand>
{
    public UnlinkParentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
        RuleFor(command => command.ParentId).NotEmpty();
    }
}
