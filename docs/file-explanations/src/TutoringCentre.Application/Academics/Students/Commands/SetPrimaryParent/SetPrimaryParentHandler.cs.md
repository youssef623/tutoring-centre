# `src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the Domain invariant (exactly one primary, and only among already-linked parents).

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,133 bytes. Source hash: `9182376393e7ad6338c7520004cc5c1f0918df7cbdb140b83e0bf060e7fcb43c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs#L9): `internal sealed class SetPrimaryParentHandler(IStudentRepository students) : ICommandHandler<SetPrimaryParentCommand, Unit>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs#L11): `public async Task<Result<Unit>> HandleAsync(SetPrimaryParentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:115](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L115)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:173](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L173)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;

/// <summary>Orchestrates: scoped lookup → the Domain invariant (exactly one primary, and only among already-linked parents).</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SetPrimaryParentHandler(IStudentRepository students) : ICommandHandler<SetPrimaryParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(SetPrimaryParentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var updated = student.SetPrimaryParent(command.ParentId);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
