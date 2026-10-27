# `src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the reverse state transition, a separate intention from archive with its own rule and error.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,113 bytes. Source hash: `80a9fd25c47bc2ea08eb00c7b4c611ace5c2b562289077a419d3aba47a92a6c1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs#L9): `internal sealed class RestoreTeacherHandler(ITeacherRepository teachers) : ICommandHandler<RestoreTeacherCommand, Unit>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs#L11): `public async Task<Result<Unit>> HandleAsync(RestoreTeacherCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:236](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L236)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;

/// <summary>Orchestrates: scoped lookup → the reverse state transition, a separate intention from archive with its own rule and error.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreTeacherHandler(ITeacherRepository teachers) : ICommandHandler<RestoreTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreTeacherCommand command, CancellationToken cancellationToken)
    {
        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        var restored = teacher.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
