using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class CreatePortalAccountHandler(ICurrentActor actor, IPortalDirectory directory,
    IStaffAccountService accounts, IMembershipRepository memberships, IPortalLinkRepository links)
    : ICommandHandler<CreatePortalAccountCommand, CreatePortalAccountResult>
{
    public async Task<Result<CreatePortalAccountResult>> HandleAsync(CreatePortalAccountCommand command, CancellationToken cancellationToken)
    {
        var centreId = actor.Actor.CentreId!.Value;
        var createdBy = actor.Actor switch { IUserActor creator => creator.UserId, SystemActor system => system.InitiatedBy, _ => null };
        if (createdBy is null || createdBy == Guid.Empty)
            return Result<CreatePortalAccountResult>.Failure(Error.Unauthenticated("auth.not_authenticated", "A signed-in creator is required."));

        var subject = command.Kind == PortalSubjectKind.Parent
            ? await directory.GetParentAsync(command.SubjectId, cancellationToken)
            : await directory.GetStudentAsync(command.SubjectId, cancellationToken);
        if (subject is not { IsActive: true })
            return Result<CreatePortalAccountResult>.Failure(PortalErrors.SubjectNotFound());
        if (await links.SubjectExistsAsync(centreId, command.Kind, command.SubjectId, cancellationToken))
            return Result<CreatePortalAccountResult>.Failure(PortalErrors.SubjectAlreadyLinked());

        var ensured = await accounts.EnsureAccountAsync(command.Email, subject.DisplayName,
            command.PreferredLocale.ToString().ToLowerInvariant(), cancellationToken);
        if (ensured.IsFailure)
            return Result<CreatePortalAccountResult>.Failure(ensured.Error!);
        var account = ensured.Value;
        if (await memberships.ExistsForUserAsync(account.UserId, centreId, cancellationToken))
            return Result<CreatePortalAccountResult>.Failure(PortalErrors.UserAlreadyLinked());

        var role = command.Kind == PortalSubjectKind.Parent ? PortalRole.Parent : PortalRole.Student;
        var membership = Membership.CreatePortal(account.UserId, centreId, role);
        if (membership.IsFailure)
            return Result<CreatePortalAccountResult>.Failure(membership.Error!);
        var link = command.Kind == PortalSubjectKind.Parent
            ? PortalLink.CreateForParent(account.UserId, centreId, command.SubjectId, createdBy.Value)
            : PortalLink.CreateForStudent(account.UserId, centreId, command.SubjectId, createdBy.Value);
        if (link.IsFailure)
            return Result<CreatePortalAccountResult>.Failure(link.Error!);

        memberships.Add(membership.Value);
        links.Add(link.Value);
        return Result<CreatePortalAccountResult>.Success(new(link.Value.Id, membership.Value.Id, account.UserId, account.TemporaryPassword));
    }
}
