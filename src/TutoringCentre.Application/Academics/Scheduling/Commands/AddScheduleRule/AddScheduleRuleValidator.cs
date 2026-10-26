using FluentValidation;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

/// <summary>
/// Shape only. The duration's range and step, the start time's boundary and the room's length are the entity's own
/// rules (<see cref="ScheduleRule.Create"/>), so they answer with its codes (<c>schedule.duration_invalid</c>,
/// <c>schedule.start_time_invalid</c>, <c>schedule.room_invalid</c>) on the same fields; repeating them here would
/// answer the same input with the generic <c>validation.failed</c> instead.
/// </summary>
internal sealed class AddScheduleRuleValidator : AbstractValidator<AddScheduleRuleCommand>
{
    public AddScheduleRuleValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.DayOfWeek).IsInEnum();
        RuleFor(command => command.EffectiveFrom).NotEqual(default(DateOnly));
    }
}
