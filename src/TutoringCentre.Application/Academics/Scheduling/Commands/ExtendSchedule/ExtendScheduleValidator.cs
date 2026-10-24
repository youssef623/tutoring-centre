using FluentValidation;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;

internal sealed class ExtendScheduleValidator : AbstractValidator<ExtendScheduleCommand>
{
    public ExtendScheduleValidator()
    {
        RuleFor(command => command.Through).NotEqual(default(DateOnly));
    }
}
