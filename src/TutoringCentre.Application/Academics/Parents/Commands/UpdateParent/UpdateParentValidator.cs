using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;

/// <summary>Shape checks only. The name's cleaning lives in Parent.Update; the phone's real validity lives in PhoneNumber.Create.</summary>
internal sealed class UpdateParentValidator : AbstractValidator<UpdateParentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public UpdateParentValidator()
    {
        RuleFor(command => command.ParentId).NotEmpty();
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Parent.NameMaxLength);
        RuleFor(command => command.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength);
    }
}
