# `src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs`

## Purpose

Reads profile, memberships, active-centre membership, password gate, and cookie session state. These identity rows are globally readable by the runtime role, so each operation explicitly filters the requested user. Session state includes the current role as well as security stamp and membership activity, allowing stale role claims to be rejected.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 3,592 bytes. Source hash: `f71068c0902b8bea8c1ce6bfa4ef8e8391eed1d8d3428f0f729652f386bf1696`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Identity;`
- Line 4: `using TutoringCentre.Domain.Centres;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L16): `internal sealed class MembershipReadService(AppDbContext db) : IMembershipReadService`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L18): `public async Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct)`
- [Line 41](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L41): `public async Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>`
- [Line 49](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L49): `public async Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct)`
- [Line 77](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L77): `public Task<bool> MustChangePasswordAsync(Guid userId, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:11](../../../../onboarding-work/extend_backend_purposes.py#L11)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:155](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L155)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of a staff member's own profile, memberships and session state. Non-tracking projections only;
/// every query filters by the given user ID, and profile/active-membership results include only active memberships.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class MembershipReadService(AppDbContext db) : IMembershipReadService
{
    public async Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.DisplayName, u.Email, u.PreferredLocale, u.MustChangePassword })
            .SingleOrDefaultAsync(ct);

        if (user is null)
        {
            return null;
        }

        var memberships = await (
            from membership in db.Set<Membership>().AsNoTracking()
            where membership.UserId == userId && membership.Status == MembershipStatus.Active
            join centre in db.Set<Centre>().AsNoTracking() on membership.CentreId equals centre.Id
            select new MembershipDto(centre.Id, centre.Name, centre.Slug, membership.Role, centre.TimeZoneId))
            .ToListAsync(ct);

        return new StaffProfileDto(user.Id, user.DisplayName, user.Email ?? string.Empty, user.PreferredLocale, memberships, user.MustChangePassword);
    }

    public async Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        await (
            from membership in db.Set<Membership>().AsNoTracking()
            where membership.UserId == userId && membership.CentreId == centreId && membership.Status == MembershipStatus.Active
            join centre in db.Set<Centre>().AsNoTracking() on membership.CentreId equals centre.Id
            select new ActiveMembershipDto(centre.Id, centre.Name, membership.Role))
            .SingleOrDefaultAsync(ct);

    public async Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct)
    {
        var stamp = await db.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.SecurityStamp)
            .SingleOrDefaultAsync(ct);

        if (stamp is null)
        {
            return null;
        }

        if (centreId is null)
        {
            return new SessionStateDto(stamp, MembershipActive: true, CurrentRole: null);
        }

        var membership = await db.Set<Membership>()
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.CentreId == centreId)
            .Select(m => new { m.Status, m.Role })
            .SingleOrDefaultAsync(ct);

        var membershipActive = membership is not null && membership.Status == MembershipStatus.Active;
        return new SessionStateDto(stamp, membershipActive, membership?.Role);
    }

    public Task<bool> MustChangePasswordAsync(Guid userId, CancellationToken ct) =>
        db.Set<ApplicationUser>().AsNoTracking().Where(u => u.Id == userId).Select(u => u.MustChangePassword).SingleOrDefaultAsync(ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
