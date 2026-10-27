# `src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: Domain invariants → set-based uniqueness → add. The dispatcher saves and commits.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,880 bytes. Source hash: `df309cabe16d2e8bb3d263fdfb87af981e5c098c665ee2dbfea45aafde27c6db`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L11): `internal sealed class CreateSubjectHandler(ICurrentActor currentActor, ISubjectRepository subjects)`
- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L14): `public async Task<Result<CreateSubjectResult>> HandleAsync(CreateSubjectCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs:21](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L21)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:183](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L183)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:115](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L115)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

/// <summary>Orchestrates: Domain invariants → set-based uniqueness → add. The dispatcher saves and commits.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateSubjectHandler(ICurrentActor currentActor, ISubjectRepository subjects)
    : ICommandHandler<CreateSubjectCommand, CreateSubjectResult>
{
    public async Task<Result<CreateSubjectResult>> HandleAsync(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var created = Subject.Create(centreId, command.Name);
        if (created.IsFailure)
        {
            return Result<CreateSubjectResult>.Failure(created.Error!);
        }

        // Set-based rule: an invariant the entity alone cannot check, backed by the unique index (Day 23).
        if (await subjects.NameExistsAsync(created.Value.NormalizedName, excludingId: null, cancellationToken))
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] };
            return Result<CreateSubjectResult>.Failure(
                new Error("subject.name_taken", "A subject with this name already exists.", ErrorKind.Conflict, fields));
        }

        subjects.Add(created.Value);
        return Result<CreateSubjectResult>.Success(new CreateSubjectResult(created.Value.Id));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
