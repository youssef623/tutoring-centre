using FluentValidation;

namespace TutoringCentre.Application.Staff.Commands.CreateStaff;

/// <summary>Shape checks only. The account's own email validity is enforced again, independently, by Identity (staff.email_invalid).</summary>
internal sealed class CreateStaffValidator : AbstractValidator<CreateStaffCommand>
{
    private const int EmailMaxLength = 256;
    private const int DisplayNameMaxLength = 120;

    public CreateStaffValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(EmailMaxLength);
        RuleFor(command => command.DisplayName).NotEmpty().MaximumLength(DisplayNameMaxLength);
        RuleFor(command => command.Role).IsInEnum();
        RuleFor(command => command.PreferredLocale).Must(locale => locale is "ar" or "en").WithMessage("Preferred locale must be 'ar' or 'en'.");
    }
}
