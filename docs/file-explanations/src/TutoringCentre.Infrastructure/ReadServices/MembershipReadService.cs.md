# src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs

## Purpose

Read-only EF projections for a staff member's profile, active memberships and session state.

## Where It Fits

Infrastructure/ReadServices, `internal`, scoped. Implements `IMembershipReadService`; used by the three Identity query handlers.

## Walkthrough

Primary constructor `MembershipReadService(AppDbContext db)`. All queries use `AsNoTracking()` and filter by the given user id.
- `GetProfileAsync` (18-39): user projection `{Id, DisplayName, Email, PreferredLocale}` with `SingleOrDefaultAsync`; null -> null; then a LINQ join `from membership in Memberships where UserId == userId && Status == Active join centre in Centres on membership.CentreId equals centre.Id select new MembershipDto(centre.Id, centre.Name, centre.Slug, membership.Role)` (`ToListAsync`).
- `GetActiveMembershipAsync` (41-47): same join filtered by user *and* centre and Active, `SingleOrDefaultAsync` -> `ActiveMembershipDto` or null.
- `GetSessionStateAsync` (49-67): reads the user's `SecurityStamp` (null -> null), then `membershipActive = centreId is null || await Memberships.AnyAsync(user, centre, Active)`.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Read service.

#### How it works here

Class.

#### Why it matters here

DTOs straight from SQL; no entities leave.

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Projections, joins, no tracking.

#### How it works here

`Select`, `join`, `AsNoTracking`.

#### Why it matters here

The query translates to SQL joins on `identity.memberships` and `platform.centres`; `Select` into a DTO fetches only needed columns.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

A membership for a deleted/missing centre is impossible by the foreign key, so the inner join cannot silently drop rows.

## Related Files

- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../TutoringCentre.Application/Identity/IMembershipReadService.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs`](../Persistence/Configurations/Identity/MembershipConfiguration.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs`](../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs.md)
