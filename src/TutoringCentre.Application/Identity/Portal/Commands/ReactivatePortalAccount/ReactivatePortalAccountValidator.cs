using FluentValidation;

namespace TutoringCentre.Application.Identity.Portal.Commands.ReactivatePortalAccount;

internal sealed class ReactivatePortalAccountValidator : AbstractValidator<ReactivatePortalAccountCommand>
{
    public ReactivatePortalAccountValidator() => RuleFor(command => command.LinkId).NotEmpty();
}
