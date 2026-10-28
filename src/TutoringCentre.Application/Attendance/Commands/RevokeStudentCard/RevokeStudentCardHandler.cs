using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Commands.RevokeStudentCard;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class RevokeStudentCardHandler(IStudentCardRepository cards, ICurrentActor actor, IClock clock)
    : ICommandHandler<RevokeStudentCardCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RevokeStudentCardCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not StaffActor staff)
            return Result<Unit>.Failure(Error.Forbidden("auth.permission_denied", "Card management requires a staff user."));
        var card = await cards.GetByIdAsync(command.CardId, command.Version, cancellationToken);
        if (card is null) { return Result<Unit>.Failure(Error.NotFound("card.not_found", "The card was not found.")); }
        var result = card.Revoke(command.Reason, staff.UserId, clock.UtcNow);
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
