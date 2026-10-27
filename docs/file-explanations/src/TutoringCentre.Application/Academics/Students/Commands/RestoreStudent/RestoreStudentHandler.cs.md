# `src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the state transition, which is behaviour on the entity, not a flag update.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,095 bytes. Source hash: `c8d3969203e1ee2956280566e3e353c18df11ecfe0224d246914753c40ee26f9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs#L9): `internal sealed class RestoreStudentHandler(IStudentRepository students) : ICommandHandler<RestoreStudentCommand, Unit>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs#L11): `public async Task<Result<Unit>> HandleAsync(RestoreStudentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:96](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L96)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:170](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L170)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;

/// <summary>Orchestrates: scoped lookup → the state transition, which is behaviour on the entity, not a flag update.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreStudentHandler(IStudentRepository students) : ICommandHandler<RestoreStudentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreStudentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var restored = student.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
