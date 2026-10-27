# `src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs`

## Purpose

Transfer loads the versioned active source, rejects the same destination, locks only target capacity, requires a matching subject, and checks duplicate/capacity rules. One timestamp ends the source as Transferred and starts the destination. Only the destination needs a capacity lock, avoiding opposing-transfer lock cycles.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 94 lines and 4,187 bytes. Source hash: `637d5c491400619cc889bafdf7afca6fc6c790c1817662da4fc66bbf56048866`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Ports;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L20): `internal sealed class TransferEnrollmentHandler(`
- [Line 27](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L27): `public async Task<Result<TransferEnrollmentResult>> HandleAsync(TransferEnrollmentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:158](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L158)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:130](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L130)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;

/// <summary>
/// Orchestrates: scoped lookup of the enrollment → still active → a different group → lock and count the target
/// → same subject as the source → not already in the target → the capacity rule on the target → end the old
/// enrollment as Transferred and start a new one. Both changes are saved in the dispatcher's one transaction, so
/// they commit together or not at all. Only the target is locked: only its capacity is at risk, and taking one
/// lock per command means two opposite transfers can never wait on each other in a cycle. The old enrollment's
/// GroupId is never changed; history stays as it was.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TransferEnrollmentHandler(
    ICurrentActor currentActor,
    IClock clock,
    IGroupRepository groups,
    IEnrollmentRepository enrollments)
    : ICommandHandler<TransferEnrollmentCommand, TransferEnrollmentResult>
{
    public async Task<Result<TransferEnrollmentResult>> HandleAsync(TransferEnrollmentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        // A stale version is detected by the save (zero rows affected), which rolls back both changes.
        var current = await enrollments.GetByIdAsync(command.EnrollmentId, command.Version, cancellationToken);
        if (current is null)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.NotFound());
        }

        if (current.Status != EnrollmentStatus.Active)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.AlreadyEnded());
        }

        if (current.GroupId == command.TargetGroupId)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.SameGroup());
        }

        var target = await groups.GetForEnrollmentAsync(command.TargetGroupId, cancellationToken);
        if (target is null)
        {
            return Result<TransferEnrollmentResult>.Failure(GroupErrors.NotFound());
        }

        // Read without a lock: the source only loses a seat, which can never break its capacity.
        var source = await groups.FindAsync(current.GroupId, cancellationToken);
        if (source is null)
        {
            return Result<TransferEnrollmentResult>.Failure(GroupErrors.NotFound());
        }

        if (source.SubjectId != target.Group.SubjectId)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.SubjectMismatch());
        }

        if (await enrollments.ActiveExistsAsync(current.StudentId, target.Group.Id, cancellationToken))
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.AlreadyEnrolled());
        }

        var canEnroll = target.Group.EnsureCanEnroll(target.ActiveEnrollmentCount);
        if (canEnroll.IsFailure)
        {
            return Result<TransferEnrollmentResult>.Failure(canEnroll.Error!);
        }

        var now = clock.UtcNow;
        var ended = current.End(now, EnrollmentEndReason.Transferred);
        if (ended.IsFailure)
        {
            return Result<TransferEnrollmentResult>.Failure(ended.Error!);
        }

        var started = Enrollment.Start(centreId, current.StudentId, target.Group.Id, now);
        if (started.IsFailure)
        {
            return Result<TransferEnrollmentResult>.Failure(started.Error!);
        }

```

The excerpt stops at line 90; use the original file for the remaining 4 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
