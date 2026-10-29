using FluentValidation;

namespace TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;

internal sealed class RegisterChannelAccountValidator : AbstractValidator<RegisterChannelAccountCommand>
{
    public RegisterChannelAccountValidator()
    {
        RuleFor(x => x.Provider).IsInEnum();
        RuleFor(x => x.ExternalAccountId).NotEmpty().Matches("^[0-9]{1,64}\\z");
        RuleFor(x => x.DisplayPhone).NotEmpty();
    }
}
