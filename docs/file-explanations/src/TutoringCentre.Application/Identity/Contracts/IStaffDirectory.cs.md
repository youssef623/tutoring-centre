# `src/TutoringCentre.Application/Identity/Contracts/IStaffDirectory.cs`

## Purpose

Source summary (intent, not independent execution proof): What another module may ask Identity about a staff member, published instead of letting that module reach into Identity's own repositories or read services directly (`TutoringCentre.Architecture.Tests`'s module-boundary rule allows a Contracts namespace specifically for this). Returns a bool and nothing else — never a Membership or a DTO — so a caller cannot learn more about the membership than the one fact it asked for.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Identity/Contracts/IStaffDirectory.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 901 bytes. Source hash: `70e48cc2537778ea94c385be9840194a4b1dd7ce90afe5ade27f15aab69f673a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Application/Identity/Contracts/IStaffDirectory.cs#L12): `public interface IStaffDirectory`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L19)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:151](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L151)
- [src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs:16](../../../../../../src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:231](../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L231)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs:12](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs:65](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L65)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Contracts;

/// <summary>
/// What another module may ask Identity about a staff member, published instead of letting that module reach
/// into Identity's own repositories or read services directly (<see cref="TutoringCentre.Architecture.Tests"/>'s
/// module-boundary rule allows a <c>Contracts</c> namespace specifically for this). Returns a bool and nothing
/// else — never a <c>Membership</c> or a DTO — so a caller cannot learn more about the membership than the one
/// fact it asked for.
/// </summary>
public interface IStaffDirectory
{
    /// <summary>Whether <paramref name="userId"/> is an active member of <paramref name="centreId"/> holding exactly <paramref name="role"/>.</summary>
    Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
