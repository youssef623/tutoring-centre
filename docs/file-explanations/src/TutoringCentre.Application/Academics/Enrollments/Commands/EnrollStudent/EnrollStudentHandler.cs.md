# `src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs`

## Purpose

Creation locks the group and counts seats before resolving the student, checking active state and duplicates, applying the capacity rule, and tracking a new enrollment. The ordering gives foreign groups a consistent not-found result and serializes competing seat decisions. The clock supplies the start instant explicitly.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 69 lines and 2,982 bytes. Source hash: `764423063f2a2416a76a663acba46e86503030b8bb380c253ad9791c4b1a265a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Academics.Students;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Ports;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Domain.Academics;`
- Line 8: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L19): `internal sealed class EnrollStudentHandler(`
- [Line 27](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L27): `public async Task<Result<EnrollStudentResult>> HandleAsync(EnrollStudentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:128](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L128)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:156](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L156)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;

/// <summary>
/// Orchestrates, in this order (ADR 0011): lock the group and count its seats → resolve the student → not already
/// enrolled → the capacity rule on the group → start and add. The lock comes first so every later step is
/// serialized per group; the group comes before the student so a foreign group ID answers group.not_found before
/// anything about the student is revealed. The dispatcher's commit releases the lock.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EnrollStudentHandler(
    ICurrentActor currentActor,
    IClock clock,
    IGroupRepository groups,
    IStudentRepository students,
    IEnrollmentRepository enrollments)
    : ICommandHandler<EnrollStudentCommand, EnrollStudentResult>
{
    public async Task<Result<EnrollStudentResult>> HandleAsync(EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        var locked = await groups.GetForEnrollmentAsync(command.GroupId, cancellationToken);
        if (locked is null)
        {
            return Result<EnrollStudentResult>.Failure(GroupErrors.NotFound());
        }

        var student = await students.FindAsync(command.StudentId, cancellationToken);
        if (student is null)
        {
            return Result<EnrollStudentResult>.Failure(StudentErrors.NotFound());
        }

        if (student.Status == StudentStatus.Archived)
        {
            return Result<EnrollStudentResult>.Failure(EnrollmentErrors.StudentArchived());
        }

        if (await enrollments.ActiveExistsAsync(student.Id, locked.Group.Id, cancellationToken))
        {
            return Result<EnrollStudentResult>.Failure(EnrollmentErrors.AlreadyEnrolled());
        }

        var canEnroll = locked.Group.EnsureCanEnroll(locked.ActiveEnrollmentCount);
        if (canEnroll.IsFailure)
        {
            return Result<EnrollStudentResult>.Failure(canEnroll.Error!);
        }

        var started = Enrollment.Start(centreId, student.Id, locked.Group.Id, clock.UtcNow);
        if (started.IsFailure)
        {
            return Result<EnrollStudentResult>.Failure(started.Error!);
        }

        enrollments.Add(started.Value);
        return Result<EnrollStudentResult>.Success(new EnrollStudentResult(started.Value.Id));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
