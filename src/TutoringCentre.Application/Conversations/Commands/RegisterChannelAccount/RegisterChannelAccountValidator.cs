using FluentValidation;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;

internal sealed class RegisterChannelAccountValidator : AbstractValidator<RegisterChannelAccountCommand>
{
    public RegisterChannelAccountValidator()
    {
        RuleFor(x => x.Provider).IsInEnum();
        When(x => x.Provider == ChannelProvider.Web, () =>
        {
            RuleFor(x => x.ExternalAccountId).NotEmpty().MaximumLength(64).Matches("^web:[a-z0-9]+(?:-[a-z0-9]+)*\\z");
            RuleFor(x => x.DisplayPhone).Null();
        }).Otherwise(() =>
        {
            RuleFor(x => x.ExternalAccountId).NotEmpty().Matches("^[0-9]{1,64}\\z");
            RuleFor(x => x.DisplayPhone).NotEmpty();
        });
    }
}
