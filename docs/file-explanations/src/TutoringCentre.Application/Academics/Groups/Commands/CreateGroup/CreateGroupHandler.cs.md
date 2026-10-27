# `src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: resolve the subject, then the teacher, inside the acting centre → Domain invariants → set-based name uniqueness → add. Another centre's subject or teacher is simply not found here, so the composite foreign keys are a backstop and never the source of the answer.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 2,700 bytes. Source hash: `786a534415c45a3beccaeb242f00a8d3c2107cbd380664ea388c637f193ea3e0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L17): `internal sealed class CreateGroupHandler(`
- [Line 24](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L24): `public async Task<Result<CreateGroupResult>> HandleAsync(CreateGroupCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs:145](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L145)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:160](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L160)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;

/// <summary>
/// Orchestrates: resolve the subject, then the teacher, inside the acting centre → Domain invariants →
/// set-based name uniqueness → add. Another centre's subject or teacher is simply not found here, so the
/// composite foreign keys are a backstop and never the source of the answer.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateGroupHandler(
    ICurrentActor currentActor,
    IGroupRepository groups,
    ISubjectRepository subjects,
    ITeacherRepository teachers)
    : ICommandHandler<CreateGroupCommand, CreateGroupResult>
{
    public async Task<Result<CreateGroupResult>> HandleAsync(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        var subject = await subjects.FindAsync(command.SubjectId, cancellationToken);
        if (subject is null)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.SubjectNotFound());
        }

        if (subject.Status == SubjectStatus.Archived)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.SubjectArchived());
        }

        var teacher = await teachers.FindAsync(command.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.TeacherNotFound());
        }

        if (teacher.Status == TeacherStatus.Archived)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.TeacherArchived());
        }

        var created = Group.Create(centreId, command.Name, subject.Id, teacher.Id, command.Capacity);
        if (created.IsFailure)
        {
            return Result<CreateGroupResult>.Failure(created.Error!);
        }

        if (await groups.NameExistsAsync(created.Value.NormalizedName, excludingId: null, cancellationToken))
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.NameTaken());
        }

        groups.Add(created.Value);
        return Result<CreateGroupResult>.Success(new CreateGroupResult(created.Value.Id));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
