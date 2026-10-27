# `src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs`

## Purpose

Revalidates the referenced subject and teacher before restoring an archived group. Existing foreign keys establish existence and centre consistency, while these application checks require current active business state. Restoring a row therefore cannot silently revive a group with archived dependencies.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,867 bytes. Source hash: `481d9caf2408a93ba9bacffbfcc3e91a1775d6f5d046b5da714225ac7e4c154a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L15): `internal sealed class RestoreGroupHandler(IGroupRepository groups, ISubjectRepository subjects, ITeacherRepository teachers)`
- [Line 18](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L18): `public async Task<Result<Unit>> HandleAsync(RestoreGroupCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:32](../../../../../../../onboarding-work/extend_backend_purposes.py#L32)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:165](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L165)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs:91](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L91)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

/// <summary>
/// Orchestrates: scoped lookup → re-validate both references → the reverse transition. A group never comes
/// back under an archived subject or teacher, since either may have been archived while it was closed.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreGroupHandler(IGroupRepository groups, ISubjectRepository subjects, ITeacherRepository teachers)
    : ICommandHandler<RestoreGroupCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        var subject = await subjects.FindAsync(group.SubjectId, cancellationToken);
        if (subject is not { Status: SubjectStatus.Active })
        {
            return Result<Unit>.Failure(GroupErrors.SubjectArchived());
        }

        var teacher = await teachers.FindAsync(group.TeacherId, cancellationToken);
        if (teacher is not { Status: TeacherStatus.Active })
        {
            return Result<Unit>.Failure(GroupErrors.TeacherArchived());
        }

        var restored = group.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
