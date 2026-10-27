# `src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: parse the phone → Domain invariants → add. Teacher names are not unique, so there is no set-based rule here.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,807 bytes. Source hash: `5df9c7361d465f3c95be9f156a0a8d905379ad3f5bae7a7eec01ca2842d79e97`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L11): `internal sealed class CreateTeacherHandler(ICurrentActor currentActor, ITeacherRepository teachers)`
- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L14): `public Task<Result<CreateTeacherResult>> HandleAsync(CreateTeacherCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs:19](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:232](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L232)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

/// <summary>Orchestrates: parse the phone → Domain invariants → add. Teacher names are not unique, so there is no set-based rule here.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateTeacherHandler(ICurrentActor currentActor, ITeacherRepository teachers)
    : ICommandHandler<CreateTeacherCommand, CreateTeacherResult>
{
    public Task<Result<CreateTeacherResult>> HandleAsync(CreateTeacherCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedPhone = PhoneNumber.Create(command.Phone);
            if (parsedPhone.IsFailure)
            {
                return Task.FromResult(Result<CreateTeacherResult>.Failure(TeacherErrors.PhoneInvalid(parsedPhone.Error!)));
            }

            phone = parsedPhone.Value;
        }

        var created = Teacher.Create(centreId, command.FullName, phone);
        if (created.IsFailure)
        {
            return Task.FromResult(Result<CreateTeacherResult>.Failure(created.Error!));
        }

        teachers.Add(created.Value);
        return Task.FromResult(Result<CreateTeacherResult>.Success(new CreateTeacherResult(created.Value.Id)));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
