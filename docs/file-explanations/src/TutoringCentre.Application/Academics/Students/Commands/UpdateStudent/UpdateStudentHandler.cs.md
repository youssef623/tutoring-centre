# `src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs`

## Purpose

Loads the student with the supplied version and applies the aggregate update to name and optional contact fields. This is a replacement-style operation: absent optional values clear them. Phone parsing and domain validation precede final UnitOfWork save.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,712 bytes. Source hash: `6b6ef10d57a8065bf7320b46bb011a5b378b8c7d80b00365a0b0842cba54ebfd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs#L13): `internal sealed class UpdateStudentHandler(IStudentRepository students) : ICommandHandler<UpdateStudentCommand, Unit>`
- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs#L15): `public async Task<Result<Unit>> HandleAsync(UpdateStudentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:34](../../../../../../../onboarding-work/extend_backend_purposes.py#L34)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:168](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L168)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;

/// <summary>
/// Orchestrates: parse the phone → scoped lookup → Domain invariants. A student in another centre and one
/// that does not exist are indistinguishable: both student.not_found. Parent links are never touched here.
/// The handler never compares versions itself — the save detects a stale one.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateStudentHandler(IStudentRepository students) : ICommandHandler<UpdateStudentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedPhone = PhoneNumber.Create(command.Phone);
            if (parsedPhone.IsFailure)
            {
                return Result<Unit>.Failure(StudentErrors.PhoneInvalid(parsedPhone.Error!));
            }

            phone = parsedPhone.Value;
        }

        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var updated = student.UpdateDetails(command.FullName, phone, command.SchoolYear);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
