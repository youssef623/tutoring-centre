using FluentValidation;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>Shape checks only. The capacity range lives in Group.ChangeCapacity, so it answers group.capacity_invalid.</summary>
internal sealed class ChangeGroupCapacityValidator : AbstractValidator<ChangeGroupCapacityCommand>
{
    public ChangeGroupCapacityValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
    }
}
