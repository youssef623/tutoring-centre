# src/TutoringCentre.Domain/Identity/StaffRole.cs

## Purpose

Enum of the three staff roles within a centre: Owner, Teacher, Secretary.

## Where It Fits

Domain/Identity. Used by `Membership`, `StaffActor`, session claims, DTOs; persisted as `'owner'|'teacher'|'secretary'` by `MembershipConfiguration`.

## Walkthrough

`Owner, Teacher, Secretary`. Doc: a role within *one* centre (not global); Secretary is the product's 'Assistant' role (frontend label `roles.secretary` = "Assistant").

## Concepts Used

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Role per membership.

#### How it works here

Enum.

#### Why it matters here

One user can hold different roles in different centres.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Roles are stored and carried in the session but **not enforced** anywhere yet (docs: Month 2).

## Related Files

- [`src/TutoringCentre.Domain/Identity/Membership.cs`](Membership.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs`](../../TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs.md)
- [`frontend/src/i18n/locales/en/auth.json`](../../../frontend/src/i18n/locales/en/auth.json.md)
