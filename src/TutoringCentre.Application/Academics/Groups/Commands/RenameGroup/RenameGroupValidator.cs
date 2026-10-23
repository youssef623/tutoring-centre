using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>Shape checks only. The name's cleaning and real validity live in Group.Rename.</summary>
internal sealed class RenameGroupValidator : AbstractValidator<RenameGroupCommand>
{
    public RenameGroupValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Group.NameMaxLength);
    }
}
