using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Commands.IssueGroupCards;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class IssueGroupCardsHandler(IAttendanceRosterSource roster, IStudentCardRepository cards,
    ICurrentActor actor, IClock clock) : ICommandHandler<IssueGroupCardsCommand, IssuedGroupCards>
{
    public async Task<Result<IssuedGroupCards>> HandleAsync(IssueGroupCardsCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not StaffActor { CentreId: { } centreId } staff)
            return Result<IssuedGroupCards>.Failure(Error.Forbidden("auth.permission_denied", "Card management requires a staff user."));
        var group = await roster.GetCardGroupAsync(command.GroupId, cancellationToken);
        if (group is null)
            return Result<IssuedGroupCards>.Failure(Error.NotFound("group.not_found", "The group was not found."));
        var members = group.Students.ToDictionary(student => student.StudentId);
        for (var index = 0; index < command.StudentIds.Count; index++)
        {
            if (!members.ContainsKey(command.StudentIds[index]))
            {
                const string message = "This student is not currently enrolled in this group.";
                return Result<IssuedGroupCards>.Failure(new Error("card.student_not_in_group", message, ErrorKind.Rule,
                    new Dictionary<string, string[]> { [$"studentIds[{index}]"] = [message] }));
            }
        }
        var active = (await cards.GetActiveAsync(command.StudentIds.ToArray(), cancellationToken)).ToDictionary(card => card.StudentId);
        var now = clock.UtcNow;
        var issued = new List<IssuedCard>();
        foreach (var id in command.StudentIds)
        {
            var result = CardIssuance.Issue(cards, active.GetValueOrDefault(id), centreId, id, members[id].FullName, staff.UserId, now);
            if (result.IsFailure) { return Result<IssuedGroupCards>.Failure(result.Error!); }
            issued.Add(result.Value);
        }
        return Result<IssuedGroupCards>.Success(new(group.GroupId, group.GroupName, issued));
    }
}
