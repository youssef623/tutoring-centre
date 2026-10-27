# `src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs`

## Purpose

The handler ensures a login, refuses any existing membership in the acting centre, constructs an active membership, and returns IDs plus a temporary password only when applicable. One dispatch transaction covers account and membership. A pre-existing inactive membership requires reactivation rather than a duplicate create operation.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 52 lines and 2,602 bytes. Source hash: `16150f6528c0d8eacae6661af38c982025f7872161f6817030ec5599398acfdd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L15): `internal sealed class CreateStaffHandler(ICurrentActor currentActor, IStaffAccountService accountService, IMembershipRepository memberships)`
- [Line 18](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L18): `public async Task<Result<CreateStaffResult>> HandleAsync(CreateStaffCommand command, CancellationToken cancellationToken)`
- [Line 47](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L47): `private static Error AlreadyMember() => new(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs:18](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:175](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L175)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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

    // Matches ConstraintErrorCatalog's ux_memberships_centre_user mapping exactly: a lost race against a
    // second concurrent create gives the identical outcome as this pre-check, not a different one.
    private static Error AlreadyMember() => new(
        "staff.already_member",
        "This person already has a membership in this centre.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["email"] = ["This person already has a membership in this centre."] });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
