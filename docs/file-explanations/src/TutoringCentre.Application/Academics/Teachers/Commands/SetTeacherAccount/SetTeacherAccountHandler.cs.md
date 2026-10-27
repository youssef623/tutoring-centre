# `src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs`

## Purpose

The handler loads a versioned teacher, unlinks for null input, or checks an active Teacher-role membership through IStaffDirectory before linking. It rejects account reuse through both a pre-check and database unique constraint. Equivalent eligibility refusals avoid disclosing whether a supplied user is unknown, foreign, inactive, or the wrong role.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 55 lines and 2,747 bytes. Source hash: `64464f27f10e1ffc8d4c3d8eb2800d06fd8952df38577a14ddb29711d95f3f55`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Application.Identity.Contracts;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L19): `internal sealed class SetTeacherAccountHandler(ICurrentActor currentActor, ITeacherRepository teachers, IStaffDirectory staffDirectory)`
- [Line 22](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L22): `public async Task<Result<Unit>> HandleAsync(SetTeacherAccountCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs:157](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L157)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:234](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L234)
- [tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs:26](../../../../../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L26)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
