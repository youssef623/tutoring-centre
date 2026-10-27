# `src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs`

## Purpose

Loads a versioned teacher and replaces editable personal fields through domain validation. Optional phone removal is intentional, while account linkage is handled by a separate command with identity eligibility checks. Successful handler return remains provisional until persistence succeeds.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,695 bytes. Source hash: `8daff2d47b82789ed447f45956f87f9556ba086123875f990eb6e54ed8b3b537`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs#L13): `internal sealed class UpdateTeacherHandler(ITeacherRepository teachers) : ICommandHandler<UpdateTeacherCommand, Unit>`
- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs#L15): `public async Task<Result<Unit>> HandleAsync(UpdateTeacherCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:35](../../../../../../../onboarding-work/extend_backend_purposes.py#L35)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:233](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L233)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

/// <summary>
/// Orchestrates: parse the phone → scoped lookup → Domain invariants. A teacher in another centre and one
/// that does not exist are indistinguishable: both teacher.not_found. The account link is never touched here.
/// The handler never compares versions itself — the save detects a stale one.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateTeacherHandler(ITeacherRepository teachers) : ICommandHandler<UpdateTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateTeacherCommand command, CancellationToken cancellationToken)
    {
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedPhone = PhoneNumber.Create(command.Phone);
            if (parsedPhone.IsFailure)
            {
                return Result<Unit>.Failure(TeacherErrors.PhoneInvalid(parsedPhone.Error!));
            }

            phone = parsedPhone.Value;
        }

        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        var updated = teacher.UpdateDetails(command.FullName, phone);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
