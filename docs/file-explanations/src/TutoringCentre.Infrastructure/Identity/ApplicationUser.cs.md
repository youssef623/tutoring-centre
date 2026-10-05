# src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs

## Purpose

Infrastructure's persistence model for a staff user, extending ASP.NET Core Identity's `IdentityUser<Guid>`.

## Where It Fits

Infrastructure/Identity. Used by `AppDbContext`, `IdentityAuthenticationService`, `DevelopmentIdentitySeeder`, `MembershipReadService`. Not visible to Application or the Api (except composition root and CLI) - enforced by `IdentityRuleTests`.

## Walkthrough

`ApplicationUser : IdentityUser<Guid>` (9). Constructor sets `Id = Guid.CreateVersion7()` (11) - same UUIDv7 scheme as `Entity`. Extra columns: `DisplayName` (string, default empty), `PreferredLocale` (`"ar"`/`"en"`), `MustChangePassword` (bool; 'Month 2'). Inherited from Identity (framework): `Email`, `NormalizedEmail`, `PasswordHash`, `SecurityStamp`, `LockoutEnd`, `AccessFailedCount`, etc.

## Concepts Used

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

User model.

#### How it works here

Class.

#### Why it matters here

Identity's own columns handle credentials and lockout; the project adds profile fields.

### UUIDv7 identifiers

#### What it means

A UUID is a 128-bit identifier. Version 7 puts a timestamp in the leading bits, so ids sort roughly by creation time. This repo assigns ids in code at construction instead of letting the database generate them.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

Guid id.

#### How it works here

Line 11.

#### Why it matters here

Consistent identifiers.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`ApplicationUser` is a persistence type, not a Domain entity: the Domain only knows `UserId` as a `Guid` on `Membership`.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs`](../Persistence/Configurations/Identity/ApplicationUserConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs`](IdentityAuthenticationService.cs.md)
