using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Commands.RevokeStudentCard;

public sealed record RevokeStudentCardCommand(Guid CardId, CardRevocationReason Reason, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceCardsManage;
}

internal sealed class RevokeStudentCardValidator : AbstractValidator<RevokeStudentCardCommand>
{
    public RevokeStudentCardValidator()
    {
        RuleFor(command => command.CardId).NotEmpty();
        RuleFor(command => command.Reason).Must(reason => reason is CardRevocationReason.Lost
            or CardRevocationReason.Damaged or CardRevocationReason.Withdrawn).WithMessage("Choose lost, damaged or withdrawn.");
    }
}
