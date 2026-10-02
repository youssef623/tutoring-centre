using FluentValidation;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>Shape checks only. Business rules (slug format, time-zone existence) live in Centre.Create — the single source of truth.</summary>
internal sealed class CreateCentreValidator : AbstractValidator<CreateCentreCommand>
{
    private const int TimeZoneIdMaxLength = 64;

    public CreateCentreValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Centre.NameMaxLength);
        RuleFor(command => command.Slug).NotEmpty().MaximumLength(Centre.SlugMaxLength);
        RuleFor(command => command.TimeZoneId).NotEmpty().MaximumLength(TimeZoneIdMaxLength);
        RuleFor(command => command.DefaultLocale).IsInEnum();
    }
}
