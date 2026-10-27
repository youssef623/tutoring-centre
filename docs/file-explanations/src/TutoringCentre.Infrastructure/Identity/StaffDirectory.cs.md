# `src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs`

## Purpose

Source summary (intent, not independent execution proof): EF Core implementation of `IStaffDirectory`. The centre condition is written by hand, like `TutoringCentre.Infrastructure.ReadServices.StaffReadService`'s: identity.memberships has no EF query filter and no row-level security (Day 29's documented exemption — a membership is read at login, before a tenant is selected), so nothing underneath this query narrows it to one centre automatically.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,391 bytes. Source hash: `0c633ac5cb702a4de9701602a98904a03b687032158bedc8405f1db8b9b10f3e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Identity.Contracts;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs#L16): `internal sealed class StaffDirectory(AppDbContext db) : IStaffDirectory`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs#L18): `public Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:151](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L151)
- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs:54](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L54)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// EF Core implementation of <see cref="IStaffDirectory"/>. The centre condition is written by hand, like
/// <see cref="TutoringCentre.Infrastructure.ReadServices.StaffReadService"/>'s: identity.memberships has no EF
/// query filter and no row-level security (Day 29's documented exemption — a membership is read at login,
/// before a tenant is selected), so nothing underneath this query narrows it to one centre automatically.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StaffDirectory(AppDbContext db) : IStaffDirectory
{
    public Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct) =>
        db.Set<Membership>()
            .AsNoTracking()
            .AnyAsync(
                membership => membership.CentreId == centreId
                    && membership.UserId == userId
                    && membership.Role == role
                    && membership.Status == MembershipStatus.Active,
                ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
