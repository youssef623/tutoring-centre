# `src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the state transition, the opposite of Archive. No uniqueness check needed — an archived subject keeps its name reserved by the unique index.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,210 bytes. Source hash: `e92564075ce03b00078503613a48ecb3579bb3a20395c92a55a236b9dd650968`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs#L9): `internal sealed class RestoreSubjectHandler(ISubjectRepository subjects) : ICommandHandler<RestoreSubjectCommand, Unit>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs#L11): `public async Task<Result<Unit>> HandleAsync(RestoreSubjectCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:186](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L186)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:118](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L118)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;

/// <summary>Orchestrates: scoped lookup → the state transition, the opposite of Archive. No uniqueness check needed — an archived subject keeps its name reserved by the unique index.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreSubjectHandler(ISubjectRepository subjects) : ICommandHandler<RestoreSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await subjects.GetByIdAsync(command.SubjectId, command.Version, cancellationToken);
        if (subject is null)
        {
            return Result<Unit>.Failure(Error.NotFound("subject.not_found", "This subject could not be found."));
        }

        var restored = subject.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
