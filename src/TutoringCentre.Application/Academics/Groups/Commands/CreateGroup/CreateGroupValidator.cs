using TutoringCentre.Domain.Common;
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;

/// <summary>Shape checks only. The name's cleaning and the capacity range live in Group.Create — the single source of truth.</summary>
internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(command => command.MonthlyFee).Must(value => value >= 0 && TutoringCentre.Domain.Common.Money.Create(value).IsSuccess).WithMessage("Use a nonnegative fee with at most two decimal places.");
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Group.NameMaxLength);
        RuleFor(command => command.SubjectId).NotEmpty();
        RuleFor(command => command.TeacherId).NotEmpty();
    }
}
