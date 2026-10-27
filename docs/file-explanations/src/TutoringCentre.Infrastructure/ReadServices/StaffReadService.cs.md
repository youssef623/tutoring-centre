# `src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs`

## Purpose

Lists centre memberships joined to user display name and email, including role, status, version, and the self marker. Membership has no implicit tenant protection, so the explicit centre predicate is mandatory. The application permission check protects this staff-specific exposure of email.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,655 bytes. Source hash: `7105295517d032e6088dbacf2a9f087d6d4bb39ecc4a7ae5d8e1949772a418e2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Staff;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs#L16): `internal sealed class StaffReadService(AppDbContext db) : IStaffReadService`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs#L18): `public async Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct) =>`
- [Line 21](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs#L21): `private IQueryable<StaffMemberDto> Query(Guid centreId, Guid currentUserId) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:13](../../../../onboarding-work/extend_backend_purposes.py#L13)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:150](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L150)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of staff listing. Unlike <see cref="SubjectReadService"/>, the centre predicate is written by
/// hand below: identity.memberships has no EF query filter and no row-level security (the Day 29 contract's
/// documented exemption — a membership is read at login, before a tenant is selected).
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StaffReadService(AppDbContext db) : IStaffReadService
{
    public async Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct) =>
        await Query(centreId, currentUserId).ToListAsync(ct);

    private IQueryable<StaffMemberDto> Query(Guid centreId, Guid currentUserId) =>
        from membership in db.Set<Membership>().AsNoTracking()
        where membership.CentreId == centreId
        join user in db.Set<ApplicationUser>().AsNoTracking() on membership.UserId equals user.Id
        orderby user.DisplayName
        select new StaffMemberDto(
            membership.Id,
            user.Id,
            user.DisplayName,
            user.Email!,
            membership.Role,
            membership.Status,
            EF.Property<uint>(membership, "Version"),
            user.Id == currentUserId);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
