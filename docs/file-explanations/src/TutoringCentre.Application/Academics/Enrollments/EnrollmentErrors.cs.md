# `src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): Errors the enrollment handlers raise themselves; the entities raise their own.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 1,345 bytes. Source hash: `b6a78ee7547790a45867283c11298ece8add11f5f3543135a5a45732f79fd152`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L6): `internal static class EnrollmentErrors`
- [Line 9](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L9): `public static Error NotFound() => Error.NotFound("enrollment.not_found", "This enrollment could not be found.");`
- [Line 11](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L11): `public static Error StudentArchived() =>`
- [Line 15](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L15): `public static Error AlreadyEnrolled() =>`
- [Line 18](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L18): `public static Error AlreadyEnded() => Error.Rule("enrollment.already_ended", "This enrollment has already ended.");`
- [Line 20](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L20): `public static Error SameGroup() =>`
- [Line 23](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L23): `public static Error SubjectMismatch() =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:46](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L46)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:36](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L36)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L23)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments;

/// <summary>Errors the enrollment handlers raise themselves; the entities raise their own.</summary>
internal static class EnrollmentErrors
{
    /// <summary>The one answer for an unknown ID and another centre's enrollment.</summary>
    public static Error NotFound() => Error.NotFound("enrollment.not_found", "This enrollment could not be found.");

    public static Error StudentArchived() =>
        Error.Rule("enrollment.student_archived", "This student is archived and cannot be enrolled.");

    /// <summary>Matches the Conflict the unique-constraint catalogue reports for ux_enrollments_active_student_group, so a race loser gets the same answer.</summary>
    public static Error AlreadyEnrolled() =>
        Error.Conflict("enrollment.already_enrolled", "This student is already enrolled in this group.");

    public static Error AlreadyEnded() => Error.Rule("enrollment.already_ended", "This enrollment has already ended.");

    public static Error SameGroup() =>
        Error.Rule("enrollment.same_group", "The student is already in this group. Choose a different group.");

    public static Error SubjectMismatch() =>
        Error.Rule("enrollment.subject_mismatch", "A transfer must be to a group of the same subject.");
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
