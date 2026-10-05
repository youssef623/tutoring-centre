# tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs

## Purpose

Architecture rules that isolate ASP.NET Core Identity: Application and the Api (except Program and the CLI) must not depend on Identity types or on `TutoringCentre.Infrastructure.Identity`.

## Where It Fits

Architecture.Tests. Backs the 'Identity is an Infrastructure concern' decision documented in the class comment and in `docs/adr/0005-cookie-authentication.md`.

## Walkthrough

Class comment (lines 5-10): Identity (`UserManager`, `ApplicationUser`, password hashing) is Infrastructure; Application and Api see it only through Application ports `IAuthenticationService` and `IMembershipReadService`. Constants (lines 13-15): `TutoringCentre.Infrastructure.Identity`, `Microsoft.AspNetCore.Identity`, `TutoringCentre.Api.Cli`.
- `Application_DoesNotDependOnIdentity` (17-28): asserts the Application type list is non-empty and that none depends on either namespace.
- `Api_OutsideCompositionRootAndCli_DoesNotDependOnIdentity` (30-40): Api types except `Program` and the `Cli` namespace must not depend on either namespace. `Program` registers Identity; the CLI hosts the development seeder.

## Concepts Used

### Architecture rules beyond dependencies

#### What it means

Beyond 'who references whom', tests can assert design conventions with reflection and IL scanning: every command has exactly one handler, handlers are sealed and internal, repositories live in the right project, endpoints do not touch Infrastructure.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies](../../../PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies).)

#### Where it appears in this file

Identity isolation rule.

#### How it works here

Both tests.

#### Why it matters here

If someone injects `UserManager<ApplicationUser>` into an endpoint, the build fails; authentication stays swappable behind the two ports.

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Ports in front of Identity.

#### How it works here

The class comment.

#### Why it matters here

`IAuthenticationService` (credentials) and `IMembershipReadService` (who belongs where) are the only identity vocabulary Application knows.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Application owns the interface, Infrastructure the implementation.

#### How it works here

Whole file.

#### Why it matters here

Same dependency inversion as repositories, applied to authentication.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs`](SourceAssemblies.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs`](ArchitectureSupport.cs.md)
- [`src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs`](../../src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs.md)
- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs.md)
- [`docs/adr/0005-cookie-authentication.md`](../../docs/adr/0005-cookie-authentication.md.md)
