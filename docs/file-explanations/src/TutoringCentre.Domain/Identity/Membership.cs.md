# src/TutoringCentre.Domain/Identity/Membership.cs

## Purpose

The entity linking one user to one centre with a role and an active/inactive status.

## Where It Fits

Domain/Identity. Created by `DevelopmentIdentitySeeder` and tests; mapped by `MembershipConfiguration`; read via `MembershipReadService`. Deliberately not `ITenantOwned`.

## Walkthrough

`public sealed class Membership : Entity` (10). Private value constructor (12-18) sets `Status = Active`; private parameterless constructor for EF (21-23). Properties `UserId`, `CentreId`, `Role`, `Status` with private setters.
- `Create(userId, centreId, role)` (33-46): `Guid.Empty` user -> `membership.user_required`; `Guid.Empty` centre -> `membership.centre_required` (both `Validation`); else a new active membership.
- `Deactivate()` (48-57): already inactive -> `Result.Failure(Error.Rule("membership.already_inactive"))`; else status Inactive, `Result.Success()`.
- `Activate()` (59-68): already active -> `membership.already_active` (Rule); else Active.
Doc: not `ITenantOwned` because memberships are read at login, before a tenant is selected, so they cannot be filtered by the current tenant.

## Concepts Used

### Entities, encapsulation and factory methods

#### What it means

An *entity* has an identity that stays the same while its data changes. *Encapsulation* keeps its state behind rules (private setters, private constructors). A *factory method* is the only public way to build one, so an invalid instance cannot exist.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

Factory + guarded state changes.

#### How it works here

`Create`, `Deactivate`, `Activate`.

#### Why it matters here

Invalid memberships and double toggles are impossible; first entity with *state transitions*.

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Result for state changes.

#### How it works here

Return types of `Deactivate/Activate`.

#### Why it matters here

Business-rule failures as values (`Rule` -> HTTP 422 if ever exposed).

### UUIDv7 identifiers

#### What it means

A UUID is a 128-bit identifier. Version 7 puts a timestamp in the leading bits, so ids sort roughly by creation time. This repo assigns ids in code at construction instead of letting the database generate them.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

Id from `Entity`.

#### How it works here

Base class.

#### Why it matters here

Same UUIDv7 scheme.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Nothing calls `Activate`/`Deactivate` in product code yet (only the seeder and tests) - staff-management commands are planned for Month 2.

## Related Files

- [`src/TutoringCentre.Domain/Common/Entity.cs`](../Common/Entity.cs.md)
- [`src/TutoringCentre.Domain/Identity/StaffRole.cs`](StaffRole.cs.md)
- [`src/TutoringCentre.Domain/Identity/MembershipStatus.cs`](MembershipStatus.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs`](../../TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs.md)
- [`tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs`](../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs.md)
