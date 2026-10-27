# `src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs`

## Purpose

Source summary (intent, not independent execution proof): The signed-in staff member's own profile. `ActiveCentreId` and `ActiveRole` are null unless the actor's selected centre is still among the user's active memberships. `Permissions` is computed fresh from `ActiveRole` on every call through the role map — advice for the UI, never stored in the cookie and never the decision itself (the dispatcher's permission step is).

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 858 bytes. Source hash: `2ff62a7a62f4d049a31308cf8ee9dbbb5020ea4ec4bb6f3b9309d0bf4d8ef7ad`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs#L11): `public sealed record MeDto(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.GetMyMemberships;

/// <summary>
/// The signed-in staff member's own profile. <see cref="ActiveCentreId"/> and <see cref="ActiveRole"/> are null
/// unless the actor's selected centre is still among the user's active memberships. <see cref="Permissions"/> is
/// computed fresh from <see cref="ActiveRole"/> on every call through the role map — advice for the UI, never
/// stored in the cookie and never the decision itself (the dispatcher's permission step is).
/// </summary>
public sealed record MeDto(
    Guid UserId,
    string DisplayName,
    string Email,
    string PreferredLocale,
    bool MustChangePassword,
    Guid? ActiveCentreId,
    StaffRole? ActiveRole,
    IReadOnlyList<MembershipDto> Memberships,
    IReadOnlyList<string> Permissions);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
