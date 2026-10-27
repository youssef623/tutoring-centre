# `src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: parse the phone → scoped lookup → Domain invariants → set-based uniqueness, excluding the parent itself and only run when the phone actually changed. A parent in another centre and one that does not exist are indistinguishable: both parent.not_found. The handler never compares versions itself — the save detects a stale one.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 1,874 bytes. Source hash: `c85d4caef1c1e5fe188703a92706efe6c78403bd046bce2164527192290001b5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L14): `internal sealed class UpdateParentHandler(IParentRepository parents) : ICommandHandler<UpdateParentCommand, Unit>`
- [Line 16](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L16): `public async Task<Result<Unit>> HandleAsync(UpdateParentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:166](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L166)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;

/// <summary>
/// Orchestrates: parse the phone → scoped lookup → Domain invariants → set-based uniqueness, excluding the
/// parent itself and only run when the phone actually changed. A parent in another centre and one that does
/// not exist are indistinguishable: both parent.not_found. The handler never compares versions itself — the
/// save detects a stale one.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateParentHandler(IParentRepository parents) : ICommandHandler<UpdateParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateParentCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Result<Unit>.Failure(ParentErrors.PhoneInvalid(phone.Error!));
        }

        var parent = await parents.GetByIdAsync(command.ParentId, command.Version, cancellationToken);
        if (parent is null)
        {
            return Result<Unit>.Failure(ParentErrors.NotFound());
        }

        var previousPhone = parent.Phone;

        var updated = parent.Update(command.FullName, phone.Value);
        if (updated.IsFailure)
        {
            return Result<Unit>.Failure(updated.Error!);
        }

        if (!parent.Phone.Equals(previousPhone)
            && await parents.PhoneExistsAsync(parent.Phone, parent.Id, cancellationToken))
        {
            return Result<Unit>.Failure(ParentErrors.PhoneTaken());
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
