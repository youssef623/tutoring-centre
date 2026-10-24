using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;

/// <summary>
/// Orchestrates: scoped lookup → the cancel transition, guarded by the clock: a session that has started is
/// refused, because attendance may already reference it. The rule keeps going; the slot is free for another
/// session under the partial exclusion constraints. Audited as a tracked update.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CancelSessionHandler(IClock clock, IClassSessionRepository sessions) : ICommandHandler<CancelSessionCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(CancelSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(command.SessionId, command.Version, cancellationToken);
        if (session is null)
        {
            return Result<Unit>.Failure(ScheduleErrors.SessionNotFound());
        }

        var cancelled = session.Cancel(clock.UtcNow);
        return cancelled.IsFailure ? Result<Unit>.Failure(cancelled.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
