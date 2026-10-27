# `src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Asks whether an existing session is still valid. Runs inside the authentication pipeline before an actor exists, so it takes the user ID explicitly rather than reading it from the actor. Internal to that pipeline: no endpoint may expose it, because it would let a caller probe security stamps and membership state directly. `Role` is the role the session's cookie claims for `CentreId` (Day 29); a mismatch against the membership's current role is treated exactly like a deactivated membership.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 861 bytes. Source hash: `97031f25e70898c90d63d3b031aeed9da3184f0b5098243bce6309e2fbb7e1da`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs#L13): `public sealed record ValidateStaffSessionQuery(Guid UserId, string SecurityStamp, Guid? CentreId, StaffRole? Role) : IQuery<bool>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs:48](../../../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L48)
- [src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs:10](../../../../../../../src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs#L10)
- [tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs:19](../../../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.ValidateStaffSession;

/// <summary>
/// Asks whether an existing session is still valid. Runs inside the authentication pipeline before an actor
/// exists, so it takes the user ID explicitly rather than reading it from the actor. Internal to that pipeline:
/// no endpoint may expose it, because it would let a caller probe security stamps and membership state directly.
/// <see cref="Role"/> is the role the session's cookie claims for <see cref="CentreId"/> (Day 29); a mismatch
/// against the membership's current role is treated exactly like a deactivated membership.
/// </summary>
public sealed record ValidateStaffSessionQuery(Guid UserId, string SecurityStamp, Guid? CentreId, StaffRole? Role) : IQuery<bool>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
