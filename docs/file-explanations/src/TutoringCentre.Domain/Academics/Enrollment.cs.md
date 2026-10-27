# `src/TutoringCentre.Domain/Academics/Enrollment.cs`

## Purpose

An enrollment records a fixed student and group relationship with start time, then can end once with a reason and valid end instant. Transfer does not mutate GroupId or reactivate history; it ends one row and starts another. External state such as capacity and duplicate active seats is checked by handlers and database constraints.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 87 lines and 3,610 bytes. Source hash: `c4180d99475de31f85fb8963e2b2101a06b240c48b62424875782836931638bc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L14): `public sealed class Enrollment : Entity, ITenantOwned`
- [Line 16](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L16): `private Enrollment(Guid centreId, Guid studentId, Guid groupId, DateTimeOffset enrolledAt)`
- [Line 26](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L26): `private Enrollment()`
- [Line 49](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L49): `public static Result<Enrollment> Start(Guid centreId, Guid studentId, Guid groupId, DateTimeOffset enrolledAt)`
- [Line 70](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L70): `public Result End(DateTimeOffset endedAt, EnrollmentEndReason reason)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:60](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L60)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:85](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L85)
- [src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs:19](../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs#L19)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:14](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L14)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:21](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L21)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:86](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L86)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs:17](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L17)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs:132](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs#L132)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs:132](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs#L132)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs:129](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs#L129)
- [src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs:37](../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs#L37)
- [src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs:85](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L85)
- [src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs:17](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L17)
- [src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs:52](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L52)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs:121](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L121)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:60](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L60)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:64](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L64)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:140](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L140)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:78](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L78)
- [tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs:28](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L28)
- 4 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// "This student holds a seat in this group since this moment." Refers to the student and the group by ID only.
/// Ending is a state transition that keeps the row: history is never edited, so re-enrolling after a withdrawal
/// starts a new enrollment and a transfer is an end plus a new start — there is no Reactivate. Timestamps are
/// passed in by the caller (from the application's clock); this type never reads a clock.
/// Not this type's concern: capacity (a rule about the group and a count of other rows, Group.EnsureCanEnroll),
/// "not already enrolled" (other rows: the handler and a partial unique index), and whether the student or the
/// group is active (other aggregates: the handler). The only way to obtain one is <see cref="Start"/>.
/// </summary>
public sealed class Enrollment : Entity, ITenantOwned
{
    private Enrollment(Guid centreId, Guid studentId, Guid groupId, DateTimeOffset enrolledAt)
    {
        CentreId = centreId;
        StudentId = studentId;
        GroupId = groupId;
        EnrolledAt = enrolledAt;
        Status = EnrollmentStatus.Active;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Enrollment()
    {
    }

    /// <summary>The centre this enrollment belongs to; fixed at start and never changeable.</summary>
    public Guid CentreId { get; private set; }

    /// <summary>The enrolled student; fixed at start and never changeable.</summary>
    public Guid StudentId { get; private set; }

    /// <summary>The group the seat is in; fixed at start and never changeable (a transfer is a new enrollment).</summary>
    public Guid GroupId { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public DateTimeOffset EnrolledAt { get; private set; }

    /// <summary>When the enrollment ended; null while active.</summary>
    public DateTimeOffset? EndedAt { get; private set; }

    /// <summary>Why the enrollment ended; null while active.</summary>
    public EnrollmentEndReason? EndReason { get; private set; }

    public static Result<Enrollment> Start(Guid centreId, Guid studentId, Guid groupId, DateTimeOffset enrolledAt)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(Error.Validation("enrollment.centre_required", "A centre is required."));
        }

        if (studentId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(Error.Validation("enrollment.student_required", "A student is required."));
        }

        if (groupId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(Error.Validation("enrollment.group_required", "A group is required."));
        }

        return Result<Enrollment>.Success(new Enrollment(centreId, studentId, groupId, enrolledAt));
    }

    /// <summary>Ends the enrollment once. A second end fails and keeps the first end's time and reason.</summary>
    public Result End(DateTimeOffset endedAt, EnrollmentEndReason reason)
    {
        if (Status == EnrollmentStatus.Ended)
        {
            return Result.Failure(Error.Rule("enrollment.already_ended", "This enrollment has already ended."));
        }

        if (endedAt < EnrolledAt)
        {
            return Result.Failure(Error.Rule("enrollment.end_before_start", "An enrollment cannot end before it started."));
        }

        Status = EnrollmentStatus.Ended;
        EndedAt = endedAt;
        EndReason = reason;
        return Result.Success();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
