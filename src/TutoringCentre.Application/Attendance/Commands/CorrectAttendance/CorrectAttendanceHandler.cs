using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Commands.CorrectAttendance;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CorrectAttendanceHandler(IAttendanceRecordRepository records, IAttendanceRosterSource roster,
    ITeachingScope teachingScope, ICurrentActor actor, IClock clock) : ICommandHandler<CorrectAttendanceCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(CorrectAttendanceCommand command, CancellationToken cancellationToken)
    {
        var record = await records.GetByIdAsync(command.RecordId, command.Version, cancellationToken);
        if (record is null)
        {
            return Result<Unit>.Failure(AttendanceErrors.NotFound());
        }

        var session = await roster.GetSessionAsync(record.SessionId, cancellationToken);
        if (session is null || !(await teachingScope.GetAsync(cancellationToken)).Allows(session.TeacherId))
        {
            return Result<Unit>.Failure(AttendanceErrors.NotFound());
        }

        if (actor.Actor is not StaffActor staff)
        {
            return Result<Unit>.Failure(Error.Forbidden("auth.permission_denied", "A correction requires an accountable staff user."));
        }

        if (!records.IsAtVersion(record, command.Version))
        {
            return Result<Unit>.Failure(Error.Conflict("concurrency.stale", "The record has changed. Reload it and try again."));
        }

        var result = record.Correct(command.Status, command.Reason, staff.UserId,
            new AttendanceSessionTiming(session.StartsAt, session.EndsAt, session.IsCancelled), clock.UtcNow);
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
