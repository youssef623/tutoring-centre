# `src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: parse the phone → set-based uniqueness → Domain invariants → add. The dispatcher saves and commits.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,863 bytes. Source hash: `95b83a9ae6fc4509cf98b2fc20bb73508147ef67342f23eed88c1f6609d2be44`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L11): `internal sealed class CreateParentHandler(ICurrentActor currentActor, IParentRepository parents)`
- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L14): `public async Task<Result<CreateParentResult>> HandleAsync(CreateParentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs:20](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L20)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:165](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L165)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

/// <summary>Orchestrates: parse the phone → set-based uniqueness → Domain invariants → add. The dispatcher saves and commits.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateParentHandler(ICurrentActor currentActor, IParentRepository parents)
    : ICommandHandler<CreateParentCommand, CreateParentResult>
{
    public async Task<Result<CreateParentResult>> HandleAsync(CreateParentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Result<CreateParentResult>.Failure(ParentErrors.PhoneInvalid(phone.Error!));
        }

        // Set-based rule: an invariant the entity alone cannot check, backed by the unique index.
        if (await parents.PhoneExistsAsync(phone.Value, excludingId: null, cancellationToken))
        {
            return Result<CreateParentResult>.Failure(ParentErrors.PhoneTaken());
        }

        var created = Parent.Create(centreId, command.FullName, phone.Value);
        if (created.IsFailure)
        {
            return Result<CreateParentResult>.Failure(created.Error!);
        }

        parents.Add(created.Value);
        return Result<CreateParentResult>.Success(new CreateParentResult(created.Value.Id));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
