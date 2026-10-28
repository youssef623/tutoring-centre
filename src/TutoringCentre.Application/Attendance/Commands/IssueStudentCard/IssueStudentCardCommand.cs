using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Commands.IssueStudentCard;

public sealed record IssueStudentCardCommand(Guid StudentId) : ICommand<IssuedCard>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceCardsManage;
}

internal sealed class IssueStudentCardValidator : AbstractValidator<IssueStudentCardCommand>
{
    public IssueStudentCardValidator() => RuleFor(command => command.StudentId).NotEmpty();
}
