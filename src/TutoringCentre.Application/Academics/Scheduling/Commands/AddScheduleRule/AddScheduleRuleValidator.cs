using FluentValidation;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

internal sealed class AddScheduleRuleValidator : AbstractValidator<AddScheduleRuleCommand>
{
    public AddScheduleRuleValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.DayOfWeek).IsInEnum();
        RuleFor(command => command.DurationMinutes)
            .InclusiveBetween(ScheduleRule.MinDurationMinutes, ScheduleRule.MaxDurationMinutes);
        RuleFor(command => command.Room).MaximumLength(ScheduleRule.RoomMaxLength);
        RuleFor(command => command.EffectiveFrom).NotEqual(default(DateOnly));
    }
}
