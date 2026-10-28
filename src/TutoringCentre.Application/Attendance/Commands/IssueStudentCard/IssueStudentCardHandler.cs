using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Commands.IssueStudentCard;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class IssueStudentCardHandler(IAttendanceRosterSource roster, IStudentCardRepository cards,
    ICurrentActor actor, IClock clock) : ICommandHandler<IssueStudentCardCommand, IssuedCard>
{
    public async Task<Result<IssuedCard>> HandleAsync(IssueStudentCardCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not StaffActor { CentreId: { } centreId } staff)
            return Result<IssuedCard>.Failure(Error.Forbidden("auth.permission_denied", "Card management requires a staff user."));
        var student = await roster.GetCardStudentAsync(command.StudentId, cancellationToken);
        if (student is null)
            return Result<IssuedCard>.Failure(Error.NotFound("student.not_found", "The student was not found."));
        if (student.IsArchived)
            return Result<IssuedCard>.Failure(Error.Rule("card.student_archived", "This student is archived and cannot use a card."));
        var active = await cards.GetActiveAsync(command.StudentId, cancellationToken);
        return CardIssuance.Issue(cards, active, centreId, student.StudentId, student.FullName, staff.UserId, clock.UtcNow);
    }
}
