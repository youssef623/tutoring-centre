# `src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs`

## Purpose

Loads a versioned teacher and refuses archival when active teaching groups remain. Archival changes business availability while retaining the optional account association and historical references. Staff membership lifecycle is separate.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,422 bytes. Source hash: `70952c29b068f3ec1c6cdb38911886a29fa68f49494237842c8bd4ab5b34de09`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs#L10): `internal sealed class ArchiveTeacherHandler(ITeacherRepository teachers, IGroupRepository groups) : ICommandHandler<ArchiveTeacherCommand, Unit>`
- [Line 12](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs#L12): `public async Task<Result<Unit>> HandleAsync(ArchiveTeacherCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:30](../../../../../../../onboarding-work/extend_backend_purposes.py#L30)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:235](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L235)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

/// <summary>Orchestrates: scoped lookup → the state transition, which is behaviour on the entity, not a flag update. The account link is untouched.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveTeacherHandler(ITeacherRepository teachers, IGroupRepository groups) : ICommandHandler<ArchiveTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveTeacherCommand command, CancellationToken cancellationToken)
    {
        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        if (await groups.AnyActiveForTeacherAsync(teacher.Id, cancellationToken))
        {
            return Result<Unit>.Failure(Error.Rule("teacher.has_active_groups", "This teacher has active groups."));
        }

        var archived = teacher.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
