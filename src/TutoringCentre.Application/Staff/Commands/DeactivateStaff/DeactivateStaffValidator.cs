using FluentValidation;

namespace TutoringCentre.Application.Staff.Commands.DeactivateStaff;

internal sealed class DeactivateStaffValidator : AbstractValidator<DeactivateStaffCommand>
{
    public DeactivateStaffValidator()
    {
        RuleFor(command => command.MembershipId).NotEmpty();
    }
}
