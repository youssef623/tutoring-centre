# `src/TutoringCentre.Application/Academics/Students/StudentErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): Errors shared by the student commands and queries. `PhoneNumber` itself has no concept of which form field it fills, so the handler attaches the field here, once, rather than every call site repeating the same dictionary literal. The "parent.*" variants exist only for RegisterStudentCommand's nested new-parent form — `TutoringCentre.Application.Academics.Parents.ParentErrors`'s own phone errors key the field "phone", which is wrong once that phone is a field of the nested parent, not of the command.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentErrors.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,637 bytes. Source hash: `247751e70fdaeed79b346066e82968de9639880b83fc50b21cc7cb729cf5b999`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentErrors.cs#L12): `internal static class StudentErrors`
- [Line 14](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentErrors.cs#L14): `public static Error NotFound() => Error.NotFound("student.not_found", "This student could not be found.");`
- [Line 16](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentErrors.cs#L16): `public static Error PhoneInvalid(Error phoneNumberError) => new(`
- [Line 22](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentErrors.cs#L22): `public static Error ParentPhoneInvalid(Error phoneNumberError) => new(`
- [Line 28](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentErrors.cs#L28): `public static Error ParentPhoneTaken() => new(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:41](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L41)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs:18](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:31](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L31)
- [src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>
/// Errors shared by the student commands and queries. <see cref="PhoneNumber"/> itself has no concept of which
/// form field it fills, so the handler attaches the field here, once, rather than every call site repeating
/// the same dictionary literal. The "parent.*" variants exist only for <c>RegisterStudentCommand</c>'s nested
/// new-parent form — <see cref="TutoringCentre.Application.Academics.Parents.ParentErrors"/>'s own phone errors
/// key the field "phone", which is wrong once that phone is a field of the nested parent, not of the command.
/// </summary>
internal static class StudentErrors
{
    public static Error NotFound() => Error.NotFound("student.not_found", "This student could not be found.");

    public static Error PhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["phone"] = [phoneNumberError.Message] });

    public static Error ParentPhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["parent.phone"] = [phoneNumberError.Message] });

    public static Error ParentPhoneTaken() => new(
        "parent.phone_taken",
        "A parent with this phone number already exists.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["parent.phone"] = ["A parent with this phone number already exists."] });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
