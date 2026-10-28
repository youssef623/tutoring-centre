using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Commands.IssueGroupCards;

public sealed record IssueGroupCardsCommand(Guid GroupId, IReadOnlyList<Guid> StudentIds)
    : ICommand<IssuedGroupCards>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceCardsManage;
}

internal sealed class IssueGroupCardsValidator : AbstractValidator<IssueGroupCardsCommand>
{
    public IssueGroupCardsValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.StudentIds).NotNull()
            .Must(ids => ids is { Count: >= 1 and <= 60 }).WithMessage("Provide between 1 and 60 student IDs.");
        RuleFor(command => command).Custom((command, context) =>
        {
            if (command.StudentIds is null) { return; }
            var seen = new HashSet<Guid>();
            for (var index = 0; index < command.StudentIds.Count; index++)
                if (command.StudentIds[index] == Guid.Empty || !seen.Add(command.StudentIds[index]))
                    context.AddFailure($"studentIds[{index}]", "Provide a non-empty, distinct student ID.");
        });
    }
}
