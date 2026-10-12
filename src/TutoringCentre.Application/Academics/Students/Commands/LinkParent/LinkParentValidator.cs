using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.LinkParent;

internal sealed class LinkParentValidator : AbstractValidator<LinkParentCommand>
{
    public LinkParentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
        RuleFor(command => command.ParentId).NotEmpty();
        RuleFor(command => command.Relationship).IsInEnum();
    }
}
