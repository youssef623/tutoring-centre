using FluentValidation;

namespace TutoringCentre.Application.Staff.Commands.ChangeStaffRole;

internal sealed class ChangeStaffRoleValidator : AbstractValidator<ChangeStaffRoleCommand>
{
    public ChangeStaffRoleValidator()
    {
        RuleFor(command => command.MembershipId).NotEmpty();
        RuleFor(command => command.Role).IsInEnum();
    }
}
