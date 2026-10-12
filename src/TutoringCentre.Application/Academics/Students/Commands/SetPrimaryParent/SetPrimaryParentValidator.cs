using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;

internal sealed class SetPrimaryParentValidator : AbstractValidator<SetPrimaryParentCommand>
{
    public SetPrimaryParentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
        RuleFor(command => command.ParentId).NotEmpty();
    }
}
