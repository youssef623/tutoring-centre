# `src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the Domain invariant (never below one parent, never the primary parent without first making another one primary). Removing the link's row once the aggregate agrees is Infrastructure's job (UnitOfWork.SaveChangesAsync), not this handler's — it only ever touches the aggregate.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,326 bytes. Source hash: `88dbe038458c503ced628b611dd686677fac725c0897e0fb8e5d4b428a7bb1af`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs#L13): `internal sealed class UnlinkParentHandler(IStudentRepository students) : ICommandHandler<UnlinkParentCommand, Unit>`
- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs#L15): `public async Task<Result<Unit>> HandleAsync(UnlinkParentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:70](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L70)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:172](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L172)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;

/// <summary>
/// Orchestrates: scoped lookup → the Domain invariant (never below one parent, never the primary parent
/// without first making another one primary). Removing the link's row once the aggregate agrees is
/// Infrastructure's job (UnitOfWork.SaveChangesAsync), not this handler's — it only ever touches the aggregate.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UnlinkParentHandler(IStudentRepository students) : ICommandHandler<UnlinkParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UnlinkParentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var removed = student.RemoveParent(command.ParentId);
        return removed.IsFailure ? Result<Unit>.Failure(removed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
