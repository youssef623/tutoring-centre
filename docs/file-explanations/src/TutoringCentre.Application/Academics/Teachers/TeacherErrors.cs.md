# `src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): Errors shared by the teacher commands and queries. `PhoneNumber` itself has no concept of which form field it fills, so the handler attaches the field here, once, rather than every call site repeating the same dictionary literal.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,799 bytes. Source hash: `b3df8ac678f260c4c21a75a0fd37b24adc8703e5af0f4783e62532e3a3d55693`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs#L10): `internal static class TeacherErrors`
- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs#L12): `public static Error NotFound() => Error.NotFound("teacher.not_found", "This teacher could not be found.");`
- [Line 14](../../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs#L14): `public static Error PhoneInvalid(Error phoneNumberError) => new(`
- [Line 24](../../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs#L24): `public static Error UserNotEligible() => new(`
- [Line 31](../../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs#L31): `public static Error AccountTaken() => new(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:25](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L25)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:27](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L27)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers;

/// <summary>
/// Errors shared by the teacher commands and queries. <see cref="PhoneNumber"/> itself has no concept of which
/// form field it fills, so the handler attaches the field here, once, rather than every call site repeating
/// the same dictionary literal.
/// </summary>
internal static class TeacherErrors
{
    public static Error NotFound() => Error.NotFound("teacher.not_found", "This teacher could not be found.");

    public static Error PhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["phone"] = [phoneNumberError.Message] });

    /// <summary>
    /// The one answer for every ineligible userId: unknown, another centre's user, a user with another role,
    /// and an inactive member all look the same here, so the response never discloses which case it was.
    /// </summary>
    public static Error UserNotEligible() => new(
        "teacher.user_not_eligible",
        "This person is not an eligible Teacher at this centre.",
        ErrorKind.Rule,
        new Dictionary<string, string[]> { ["userId"] = ["This person is not an eligible Teacher at this centre."] });

    /// <summary>Matches the Conflict the unique-constraint catalogue reports for ux_teachers_centre_user, so a race loser gets the same answer as the handler's own check.</summary>
    public static Error AccountTaken() => new(
        "teacher.account_taken",
        "This account is already linked to another teacher.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["userId"] = ["This account is already linked to another teacher."] });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
