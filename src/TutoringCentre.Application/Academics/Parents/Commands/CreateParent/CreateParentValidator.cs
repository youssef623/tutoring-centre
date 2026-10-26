using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

/// <summary>Shape checks only. The name's cleaning lives in Parent.Create; the phone's real validity lives in PhoneNumber.Create — both the single sources of truth.</summary>
internal sealed class CreateParentValidator : AbstractValidator<CreateParentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public CreateParentValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Parent.NameMaxLength);
        RuleFor(command => command.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength);
    }
}
