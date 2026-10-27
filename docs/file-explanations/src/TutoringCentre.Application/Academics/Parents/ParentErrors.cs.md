# `src/TutoringCentre.Application/Academics/Parents/ParentErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): Errors shared by the parent commands and queries. `PhoneNumber` itself has no concept of which form field it fills — phone.invalid reaches the handler with no field attached — so the handler attaches "phone" here, once, rather than every call site repeating the same dictionary literal.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Parents/ParentErrors.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 1,119 bytes. Source hash: `962d116b7a94a69eb24eb9da019ac98302b8c464dba53d88f5b60f99fe6d4db1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Parents/ParentErrors.cs#L10): `internal static class ParentErrors`
- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Parents/ParentErrors.cs#L12): `public static Error PhoneInvalid(Error phoneNumberError) => new(`
- [Line 18](../../../../../../src/TutoringCentre.Application/Academics/Parents/ParentErrors.cs#L18): `public static Error PhoneTaken() => new(`
- [Line 24](../../../../../../src/TutoringCentre.Application/Academics/Parents/ParentErrors.cs#L24): `public static Error NotFound() => Error.NotFound("parent.not_found", "This parent could not be found.");`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs:27](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L27)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:43](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L43)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents;

/// <summary>
/// Errors shared by the parent commands and queries. <see cref="PhoneNumber"/> itself has no concept of which
/// form field it fills — <c>phone.invalid</c> reaches the handler with no field attached — so the handler
/// attaches "phone" here, once, rather than every call site repeating the same dictionary literal.
/// </summary>
internal static class ParentErrors
{
    public static Error PhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["phone"] = [phoneNumberError.Message] });

    public static Error PhoneTaken() => new(
        "parent.phone_taken",
        "A parent with this phone number already exists.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["phone"] = ["A parent with this phone number already exists."] });

    public static Error NotFound() => Error.NotFound("parent.not_found", "This parent could not be found.");
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
