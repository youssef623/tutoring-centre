# `docs/architecture/authentication.md`

## Purpose

Documentation covering Authentication; Login; A request that already carries a session; Centre selection; Password change and the first-login gate. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/authentication.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 249 lines and 17,106 bytes. Source hash: `88e91b23696c38cf33ec0effdffa036cd7521132d528e8cd5bdbc0448ae66a98`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Authentication
- Line 8: Login
- Line 38: A request that already carries a session
- Line 48: Centre selection
- Line 61: Password change and the first-login gate
- Line 96: Every authenticated request
- Line 115: Anonymous routes
- Line 135: Logout
- Line 141: The cookie and its claims
- Line 165: CSRF
- Line 183: Revalidation and revocation
- Line 210: Lockout and rate limiting
- Line 235: Error codes

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Authentication

How staff sign in, how a session is carried and revalidated, and how the frontend defends every
state-changing request against CSRF. Written from the implementation in `src/TutoringCentre.Api/Auth/`,
`src/TutoringCentre.Application/Identity/` and `src/TutoringCentre.Infrastructure/Identity/` — every name,
code and number below is checked against that code, not against the plan that produced it.

## Login

`POST /api/auth/login` (`AuthEndpoints.LoginAsync`), anonymous, rate-limited, CSRF-protected.

1. The request body is validated (`LoginRequestValidator`); a validation failure returns 400 before any
   credential check runs.
2. Credentials are verified through a port, `IAuthenticationService.VerifyCredentialsAsync` — not a CQRS
   command, since signing in has no transaction boundary of its own. The one implementation,
   `IdentityAuthenticationService` (Infrastructure), calls ASP.NET Core Identity's `UserManager<ApplicationUser>`
   directly: it is the first of two intentional write paths outside the dispatcher pipeline, because
   `UserManager` saves its own counters (failed-attempt count, lockout end, security stamp) as it goes. The
   second is the Data Protection key ring itself (Task 34.6, `platform.data_protection_keys`): the framework
   writes a new key there on its own schedule, not through anything this document's request flow touches.
   - An unknown email still pays a real password-hash verification, against a fixed dummy user, so response
     time carries no signal about whether the account exists.
   - A locked-out user, a wrong password, and an unknown email all return the identical failure:
     `auth.invalid_credentials`, 401.
```

The excerpt stops at line 24; use the original file for the remaining 225 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
