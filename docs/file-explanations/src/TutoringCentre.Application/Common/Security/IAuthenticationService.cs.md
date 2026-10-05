# src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs

## Purpose

Application's port for verifying staff credentials, plus the `AuthenticatedUser` result record.

## Where It Fits

Application/Common/Security. Implemented by `Infrastructure/Identity/IdentityAuthenticationService`; called by `AuthEndpoints.LoginAsync`.

## Walkthrough

`Task<Result<AuthenticatedUser>> VerifyCredentialsAsync(string email, string password, CancellationToken)` and `record AuthenticatedUser(Guid UserId, string SecurityStamp)`. Doc: every failure is `Unauthenticated("auth.invalid_credentials")` with the same message, so an unknown email, a wrong password and a locked account are indistinguishable (no enumeration). It is a *port*, not a CQRS command, because sign-in has no use-case transaction of its own (ADR 0005).

## Concepts Used

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Port for Identity.

#### How it works here

Interface.

#### Why it matters here

Application and the Api never see `UserManager` or `ApplicationUser` (enforced by `IdentityRuleTests`).

### Rate limiting, lockout and enumeration resistance

#### What it means

*Rate limiting* caps how many requests one source may make in a time window (stops password spraying from one place). *Lockout* temporarily blocks one account after repeated failures (stops many guesses against one account). *Enumeration resistance* means failure responses (and their timing) reveal nothing about whether an account exists.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance](../../../../../PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance).)

#### Where it appears in this file

Enumeration resistance contract.

#### How it works here

Doc comment.

#### Why it matters here

Defines what implementations must not reveal.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Login lives outside the dispatcher pipeline, so it gets no automatic transaction/logging line.

## Related Files

- [`src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs`](../../../TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](../../../TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs`](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs.md)
