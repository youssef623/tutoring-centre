using FluentValidation;
namespace TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;
internal sealed class CreateMonthlyChargesValidator : AbstractValidator<CreateMonthlyChargesCommand>
{
    public CreateMonthlyChargesValidator()
    {
        // Adjacent local-month boundaries must be representable when converting them to UTC instants.
        RuleFor(x => x.Month).Must(x => x is null || x.Value.FirstDay.Year is > 1 and < 9999).WithMessage("The billing month is outside the supported calendar range.");
    }
}
