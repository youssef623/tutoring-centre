# `src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs`

## Purpose

Registration resolves an existing parent or constructs a new canonical-phone parent, then constructs the student and primary link within one dispatch transaction. A newly tracked parent remains provisional if later student validation fails. Nested parent dispatch is deliberately avoided so both aggregates share one save and rollback boundary.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 80 lines and 3,568 bytes. Source hash: `3ca48dd7275ce0a1384638944115c47bd1f8ddabc61e2b4f4bf0829a58bc447f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Parents;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L17): `internal sealed class RegisterStudentHandler(ICurrentActor currentActor, IStudentRepository students, IParentRepository parents)`
- [Line 20](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L20): `public async Task<Result<RegisterStudentResult>> HandleAsync(RegisterStudentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs:136](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L136)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:167](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L167)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

/// <summary>
/// Orchestrates one transaction across two aggregates: resolve or create the parent, then register the
/// student against it. Nothing is added before every check that can fail has passed, and the handler never
/// dispatches a nested command — the nested-transaction guard would refuse it, and it would be two
/// transactions instead of one. The dispatcher saves once: both aggregates and the link, or neither.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RegisterStudentHandler(ICurrentActor currentActor, IStudentRepository students, IParentRepository parents)
    : ICommandHandler<RegisterStudentCommand, RegisterStudentResult>
{
    public async Task<Result<RegisterStudentResult>> HandleAsync(RegisterStudentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        PhoneNumber? studentPhone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedStudentPhone = PhoneNumber.Create(command.Phone);
            if (parsedStudentPhone.IsFailure)
            {
                return Result<RegisterStudentResult>.Failure(StudentErrors.PhoneInvalid(parsedStudentPhone.Error!));
            }

            studentPhone = parsedStudentPhone.Value;
        }

        Guid parentId;
        if (command.Parent.ExistingParentId is { } existingParentId)
        {
            var existingParent = await parents.FindAsync(existingParentId, cancellationToken);
            if (existingParent is null)
            {
                return Result<RegisterStudentResult>.Failure(ParentErrors.NotFound());
            }

            parentId = existingParent.Id;
        }
        else
        {
            var parentPhone = PhoneNumber.Create(command.Parent.Phone!);
            if (parentPhone.IsFailure)
            {
                return Result<RegisterStudentResult>.Failure(StudentErrors.ParentPhoneInvalid(parentPhone.Error!));
            }

            if (await parents.PhoneExistsAsync(parentPhone.Value, excludingId: null, cancellationToken))
            {
                return Result<RegisterStudentResult>.Failure(StudentErrors.ParentPhoneTaken());
            }

            var createdParent = Parent.Create(centreId, command.Parent.FullName!, parentPhone.Value);
            if (createdParent.IsFailure)
            {
                return Result<RegisterStudentResult>.Failure(createdParent.Error!);
            }

            parents.Add(createdParent.Value);
            parentId = createdParent.Value.Id;
        }

        var registered = Student.Register(centreId, command.FullName, studentPhone, command.SchoolYear, parentId, command.Relationship);
        if (registered.IsFailure)
        {
            return Result<RegisterStudentResult>.Failure(registered.Error!);
        }

        students.Add(registered.Value);
        return Result<RegisterStudentResult>.Success(new RegisterStudentResult(registered.Value.Id, parentId));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
