using FluentValidation;

namespace TutoringCentre.Application.Identity.Portal.Commands.DeactivatePortalAccount;

internal sealed class DeactivatePortalAccountValidator : AbstractValidator<DeactivatePortalAccountCommand>
{
    public DeactivatePortalAccountValidator() => RuleFor(command => command.LinkId).NotEmpty();
}
