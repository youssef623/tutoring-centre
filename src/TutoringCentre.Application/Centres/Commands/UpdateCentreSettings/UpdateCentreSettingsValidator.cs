using FluentValidation;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;

/// <summary>Shape checks only. The name rule itself lives in Centre.UpdateSettings — the single source of truth.</summary>
internal sealed class UpdateCentreSettingsValidator : AbstractValidator<UpdateCentreSettingsCommand>
{
    public UpdateCentreSettingsValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Centre.NameMaxLength);
        RuleFor(command => command.DefaultLocale).IsInEnum();
    }
}
