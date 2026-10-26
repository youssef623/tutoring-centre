using FluentValidation;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;

internal sealed class CancelSessionValidator : AbstractValidator<CancelSessionCommand>
{
    public CancelSessionValidator()
    {
        RuleFor(command => command.SessionId).NotEmpty();
    }
}
