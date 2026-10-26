using FluentValidation;

namespace TutoringCentre.Application.Staff.Commands.ReactivateStaff;

internal sealed class ReactivateStaffValidator : AbstractValidator<ReactivateStaffCommand>
{
    public ReactivateStaffValidator()
    {
        RuleFor(command => command.MembershipId).NotEmpty();
    }
}
