using TutoringCentre.Domain.Common;
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>Shape checks only. The name's cleaning and real validity live in Group.Rename.</summary>
internal sealed class RenameGroupValidator : AbstractValidator<RenameGroupCommand>
{
    public RenameGroupValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.MonthlyFee).Must(value => value is null || (value >= 0 && TutoringCentre.Domain.Common.Money.Create(value.Value).IsSuccess)).WithMessage("Use a nonnegative fee with at most two decimal places.");
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Group.NameMaxLength);
    }
}
