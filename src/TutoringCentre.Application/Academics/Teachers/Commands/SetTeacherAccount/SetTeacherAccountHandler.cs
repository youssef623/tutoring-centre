using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;

/// <summary>
/// Orchestrates: scoped lookup → unlink or link. Linking asks Identity's own published contract whether the
/// user is an active Teacher member of the actor's centre — unknown user, another centre's user, a user with
/// another role and an inactive member all answer the same way, so this handler never discloses which case it
/// was. The second check (already linked to another teacher) is the handler's own set-based guard; the unique
/// index behind ux_teachers_centre_user (ConstraintErrorCatalog) answers identically for a race that slips
/// past it.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SetTeacherAccountHandler(ICurrentActor currentActor, ITeacherRepository teachers, IStaffDirectory staffDirectory)
    : ICommandHandler<SetTeacherAccountCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(SetTeacherAccountCommand command, CancellationToken cancellationToken)
    {
        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        if (command.UserId is null)
        {
            var unlinked = teacher.UnlinkUser();
            return unlinked.IsFailure ? Result<Unit>.Failure(unlinked.Error!) : Result<Unit>.Success(Unit.Value);
        }

        var userId = command.UserId.Value;
        var centreId = currentActor.Actor.CentreId!.Value;

        // The Teacher role is passed to the contract as data, not branched on for an authorization decision
        // (AuthorizationRuleTests rule 3's allow-list carries this file for that reason).
        var isEligible = await staffDirectory.IsActiveMemberWithRoleAsync(centreId, userId, StaffRole.Teacher, cancellationToken);
        if (!isEligible)
        {
            return Result<Unit>.Failure(TeacherErrors.UserNotEligible());
        }

        if (await teachers.UserLinkedAsync(userId, excludingTeacherId: teacher.Id, cancellationToken))
        {
            return Result<Unit>.Failure(TeacherErrors.AccountTaken());
        }

        var linked = teacher.LinkUser(userId);
        return linked.IsFailure ? Result<Unit>.Failure(linked.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
