using FluentValidation;

namespace TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;

internal sealed class CreatePortalAccountValidator : AbstractValidator<CreatePortalAccountCommand>
{
    public CreatePortalAccountValidator()
    {
        RuleFor(command => command.Kind).IsInEnum();
        RuleFor(command => command.SubjectId).NotEmpty();
        RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(command => command.PreferredLocale).IsInEnum();
    }
}
