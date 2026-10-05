# src/TutoringCentre.Domain/Identity/MembershipStatus.cs

## Purpose

Enum: `Active` or `Inactive` - whether a membership currently grants access.

## Where It Fits

Domain/Identity. Used by `Membership`, read services (filter `Status == Active`), persisted as `'active'|'inactive'`.

## Walkthrough

`Active, Inactive`. Deactivating a membership is how access is revoked without deleting history; the session revalidation then rejects sessions bound to it.

## Concepts Used

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Soft revocation.

#### How it works here

Enum.

#### Why it matters here

Keeps the row (and its audit meaning) while denying access.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Domain/Identity/Membership.cs`](Membership.cs.md)
- [`src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs`](../../TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs.md)
