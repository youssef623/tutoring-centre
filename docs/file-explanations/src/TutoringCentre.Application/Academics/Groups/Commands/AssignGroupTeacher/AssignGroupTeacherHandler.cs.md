# `src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs`

## Purpose

Reassignment validates a visible active teacher, locks old and new teacher schedules, changes the versioned group, and moves future scheduled sessions as tracked entities. Past and cancelled sessions retain history. Any new-teacher overlap rejects the common save and restores group and sessions together.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 67 lines and 2,901 bytes. Source hash: `9a444fc1ebc669754dc340a76a0f2dd56c496bf1719bce9de6877b1fce7eec19`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Ports;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L19): `internal sealed class AssignGroupTeacherHandler(`
- [Line 26](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L26): `public async Task<Result<Unit>> HandleAsync(AssignGroupTeacherCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:107](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L107)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:163](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L163)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;

/// <summary>
/// Orchestrates: scoped group lookup → resolve the new teacher inside the acting centre → the entity's change →
/// move the group's future scheduled sessions to the new teacher in the same transaction, so the old teacher is
/// not left booked. If the new teacher is busy at one of those times, the exclusion constraint refuses the save
/// and the whole reassignment rolls back. Past and cancelled sessions keep their teacher. No audit code: the audit
/// policy records teacherId before and after, on the group and on each moved session.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AssignGroupTeacherHandler(
    IClock clock,
    IGroupRepository groups,
    ITeacherRepository teachers,
    IClassSessionRepository sessions)
    : ICommandHandler<AssignGroupTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(AssignGroupTeacherCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        var teacher = await teachers.FindAsync(command.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(GroupErrors.TeacherNotFound());
        }

        if (teacher.Status == TeacherStatus.Archived)
        {
            return Result<Unit>.Failure(GroupErrors.TeacherArchived());
        }

        // Both teachers, before either's sessions change: a concurrent schedule write for the new teacher is then
        // refused by the constraint, not ended in a deadlock.
        await sessions.LockTeacherSchedulesAsync([group.TeacherId, teacher.Id], cancellationToken);

        var assigned = group.AssignTeacher(teacher.Id);
        if (assigned.IsFailure)
        {
            return Result<Unit>.Failure(assigned.Error!);
        }

        var now = clock.UtcNow;
        foreach (var session in await sessions.ListFutureScheduledForGroupAsync(group.Id, now, cancellationToken))
        {
            var reassigned = session.ReassignTeacher(teacher.Id, now);
            if (reassigned.IsFailure)
            {
                return Result<Unit>.Failure(reassigned.Error!);
            }
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
