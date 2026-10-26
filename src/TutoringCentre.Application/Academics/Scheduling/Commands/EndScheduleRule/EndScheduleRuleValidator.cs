using FluentValidation;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;

internal sealed class EndScheduleRuleValidator : AbstractValidator<EndScheduleRuleCommand>
{
    public EndScheduleRuleValidator()
    {
        RuleFor(command => command.ScheduleRuleId).NotEmpty();
        RuleFor(command => command.LastDate).NotEqual(default(DateOnly));
    }
}
