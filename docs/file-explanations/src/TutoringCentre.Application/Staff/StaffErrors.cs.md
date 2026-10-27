# `src/TutoringCentre.Application/Staff/StaffErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): The three outcomes shared by every staff command's decision order (Day 29 contract): scoped lookup, self-change, and the last-owner guard.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Staff/StaffErrors.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 658 bytes. Source hash: `ecbc55a9d7035f886a00bbef5368b06dc211c14ffae0427dbd95ad473abfdcbd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Application/Staff/StaffErrors.cs#L6): `internal static class StaffErrors`
- [Line 8](../../../../../src/TutoringCentre.Application/Staff/StaffErrors.cs#L8): `public static Error NotFound() => Error.NotFound("staff.not_found", "This staff member could not be found.");`
- [Line 10](../../../../../src/TutoringCentre.Application/Staff/StaffErrors.cs#L10): `public static Error CannotChangeSelf() => Error.Rule("staff.cannot_change_self", "You cannot change your own membership this way.");`
- [Line 12](../../../../../src/TutoringCentre.Application/Staff/StaffErrors.cs#L12): `public static Error LastOwner() => Error.Rule("staff.last_owner", "This centre must keep at least one active owner.");`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs:25](../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs#L25)
- [src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs:25](../../../../../src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs#L25)
- [src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs:20](../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff;

/// <summary>The three outcomes shared by every staff command's decision order (Day 29 contract): scoped lookup, self-change, and the last-owner guard.</summary>
internal static class StaffErrors
{
    public static Error NotFound() => Error.NotFound("staff.not_found", "This staff member could not be found.");

    public static Error CannotChangeSelf() => Error.Rule("staff.cannot_change_self", "You cannot change your own membership this way.");

    public static Error LastOwner() => Error.Rule("staff.last_owner", "This centre must keep at least one active owner.");
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
