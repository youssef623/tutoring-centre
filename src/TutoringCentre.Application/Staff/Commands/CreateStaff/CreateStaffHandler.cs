using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.CreateStaff;

/// <summary>
/// Orchestrates two writes behind one transaction: ensure the login account, then add the membership. A
/// person who already has a membership in this centre — active or inactive — gets staff.already_member,
/// whether or not the account itself already existed.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateStaffHandler(ICurrentActor currentActor, IStaffAccountService accountService, IMembershipRepository memberships)
    : ICommandHandler<CreateStaffCommand, CreateStaffResult>
{
    public async Task<Result<CreateStaffResult>> HandleAsync(CreateStaffCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var accountResult = await accountService.EnsureAccountAsync(command.Email, command.DisplayName, command.PreferredLocale, cancellationToken);
        if (accountResult.IsFailure)
        {
            return Result<CreateStaffResult>.Failure(accountResult.Error!);
        }

        var account = accountResult.Value;
        if (await memberships.ExistsForUserAsync(account.UserId, centreId, cancellationToken))
        {
            return Result<CreateStaffResult>.Failure(AlreadyMember());
        }

        var created = Membership.Create(account.UserId, centreId, command.Role);
        if (created.IsFailure)
        {
            return Result<CreateStaffResult>.Failure(created.Error!);
        }

        memberships.Add(created.Value);
        return Result<CreateStaffResult>.Success(new CreateStaffResult(created.Value.Id, account.UserId, account.TemporaryPassword));
    }

    // Matches UniqueConstraintCatalogue's ux_memberships_centre_user mapping exactly: a lost race against a
    // second concurrent create gives the identical outcome as this pre-check, not a different one.
    private static Error AlreadyMember() => new(
        "staff.already_member",
        "This person already has a membership in this centre.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["email"] = ["This person already has a membership in this centre."] });
}
