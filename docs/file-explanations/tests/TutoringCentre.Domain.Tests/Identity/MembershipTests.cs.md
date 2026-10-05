# tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs

## Purpose

Unit tests of the `Membership` entity: creation rules and the Activate/Deactivate state transitions.

## Where It Fits

Domain.Tests/Identity. Pure in-memory tests, no database.

## Walkthrough

Static ids (lines 7-8). Six `[Fact]`s:
- `Create_WithValidInput_SucceedsAsActiveWithTheGivenRole` (10-23): result success; `UserId`, `CentreId`, `Role == Teacher`, `Status == Active`.
- `Create_WithEmptyUserId_ReturnsUserRequired` (25-34): `membership.user_required`.
- `Create_WithEmptyCentreId_ReturnsCentreRequired` (36-45): `membership.centre_required`.
- `Deactivate_WhenActive_SucceedsAndBecomesInactive` (47-59).
- `Deactivate_WhenAlreadyInactive_ReturnsAlreadyInactive` (61-74): `membership.already_inactive`.
- `Activate_WhenAlreadyActive_ReturnsAlreadyActive` (76-88): `membership.already_active`.
Not tested here: `Activate()` on an inactive membership succeeding (only the failure branch is asserted).

## Concepts Used

### Entities, encapsulation and factory methods

#### What it means

An *entity* has an identity that stays the same while its data changes. *Encapsulation* keeps its state behind rules (private setters, private constructors). A *factory method* is the only public way to build one, so an invalid instance cannot exist.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

Factory method and guarded transitions.

#### How it works here

`Membership.Create`, `Deactivate`, `Activate`.

#### Why it matters here

The entity cannot be created invalid and refuses meaningless transitions; failures are `Result` values.

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Error codes asserted.

#### How it works here

`result.Error!.Code`.

#### Why it matters here

Stable codes like `membership.user_required` are the contract the rest of the system sees.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The successful `Activate()` path (inactive to active) has no test in this class.

## Related Files

- [`src/TutoringCentre.Domain/Identity/Membership.cs`](../../../src/TutoringCentre.Domain/Identity/Membership.cs.md)
