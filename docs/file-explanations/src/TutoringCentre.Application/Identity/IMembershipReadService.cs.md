# src/TutoringCentre.Application/Identity/IMembershipReadService.cs

## Purpose

Read-side port for a staff member's own profile, active memberships and session state, with its DTO records.

## Where It Fits

Application/Identity. Implemented by `Infrastructure/ReadServices/MembershipReadService`; consumed by `GetMyMembershipsHandler`, `GetActiveMembershipHandler`, `ValidateStaffSessionHandler`; faked by `FakeMembershipReadService`.

## Walkthrough

Methods: `GetProfileAsync(userId)` -> `StaffProfileDto?`; `GetActiveMembershipAsync(userId, centreId)` -> `ActiveMembershipDto?`; `GetSessionStateAsync(userId, centreId?)` -> `SessionStateDto?`. Doc: every operation takes the user id explicitly and returns only that user's data - never another user's, never an entity, never `IQueryable`.
Records: `StaffProfileDto(UserId, DisplayName, Email, PreferredLocale, IReadOnlyList<MembershipDto> Memberships)`; `MembershipDto(CentreId, CentreName, CentreSlug, StaffRole Role)`; `ActiveMembershipDto(CentreId, CentreName, StaffRole Role)`; `SessionStateDto(SecurityStamp, bool MembershipActive)`.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Read service port.

#### How it works here

Interface.

#### Why it matters here

Reads skip entities; shaped for the use cases.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Port in Application.

#### How it works here

Namespace `TutoringCentre.Application.Identity`.

#### Why it matters here

Identity tables stay in Infrastructure.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Returning `null` for 'not found' is the contract that lets handlers return identical failures for unknown/foreign/inactive centres.

## Related Files

- [`src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs`](../../TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs`](Queries/GetMyMemberships/GetMyMembershipsHandler.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs`](Queries/GetActiveMembership/GetActiveMembershipHandler.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs`](Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs.md)
