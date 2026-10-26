using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;

/// <summary>
/// Orchestrates: locked lookup with the active count → version check → no active enrollment → no schedule rule
/// still in effect from today (the centre's calendar) → the state transition. The row is never deleted. Takes the
/// same group lock as enrolling, so no enrollment can land between the count and the archive.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveGroupHandler(
    IClock clock,
    ICentreTimeZoneProvider timeZones,
    IGroupRepository groups,
    IScheduleRuleRepository rules)
    : ICommandHandler<ArchiveGroupCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveGroupCommand command, CancellationToken cancellationToken)
    {
        var locked = await groups.GetForEnrollmentAsync(command.GroupId, cancellationToken);
        if (locked is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        if (!groups.IsAtVersion(locked.Group, command.Version))
        {
            return Result<Unit>.Failure(GroupErrors.Stale());
        }

        if (locked.ActiveEnrollmentCount > 0)
        {
            return Result<Unit>.Failure(GroupErrors.HasActiveEnrollments());
        }

        var today = CentreCalendar.Today(clock, await timeZones.GetForCurrentCentreAsync(cancellationToken));
        if (await rules.AnyActiveForGroupAsync(locked.Group.Id, today, cancellationToken))
        {
            return Result<Unit>.Failure(GroupErrors.HasActiveSchedule());
        }

        var archived = locked.Group.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
