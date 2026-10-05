# tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs

## Purpose

Proves the credential adapter against real PostgreSQL: correct and wrong passwords, unknown email, lockout after five failures, counter reset, and that the email is never logged.

## Where It Fits

Infrastructure.Tests/Identity; uses `PostgresFixture`/`PostgresTestBase`. Resolves `IAuthenticationService` (implemented by `IdentityAuthenticationService`) and `UserManager<ApplicationUser>` from a real container-backed service provider.

## Walkthrough

Constants `Email = staff@nile.test`, `Password = a-strong-enough-password` (lines 13-14; throwaway test values). Each test makes a scope with an in-memory logger (`CreateScope` (95-96): `Fixture.CreateServiceProvider(services => services.AddLogging(builder => builder.AddProvider(log)))`).
- `VerifyCredentialsAsync_CorrectPassword_SucceedsWithUserIdAndStamp` (16-29): success; `UserId` equals the created user's id; `SecurityStamp` not blank.
- `..._WrongPassword_FailsAndRecordsOneFailedAttempt` (31-44): failure `auth.invalid_credentials`; `access_failed_count` in `identity.users` is 1 (read with `Fixture.ScalarAsync`).
- `..._UnknownEmail_FailsWithTheIdenticalError` (46-57): same code.
- `..._FiveWrongAttemptsThenCorrectPassword_StillFails` (59-77): lockout in effect.
- `..._SuccessAfterAFailure_ResetsTheCounter` (79-93): counter back to 0.
Every test ends with `AssertNoEmailLogged` (120-121): no captured log line contains `@nile.test`.
Helpers: `CreateUserAsync` (101-115) uses `UserManager.CreateAsync(user, password)` (real hashing, `EmailConfirmed = true`); private `CapturingLoggerProvider` (1) and its nested logger append formatted lines to a list.

## Concepts Used

### Rate limiting, lockout and enumeration resistance

#### What it means

*Rate limiting* caps how many requests one source may make in a time window (stops password spraying from one place). *Lockout* temporarily blocks one account after repeated failures (stops many guesses against one account). *Enumeration resistance* means failure responses (and their timing) reveal nothing about whether an account exists.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance](../../../../PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance).)

#### Where it appears in this file

Lockout and no-enumeration.

#### How it works here

Tests at 31-93.

#### Why it matters here

Failure is always the same `auth.invalid_credentials`, and five failures lock the account while still looking like a wrong password.

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Real `UserManager` and Identity tables.

#### How it works here

`CreateUserAsync` and `FailedAttemptCountAsync`.

#### Why it matters here

Identity is configured exactly as in production, against the `identity` schema.

### Testcontainers and Respawn

#### What it means

Testcontainers starts a disposable Docker container (here PostgreSQL 17) for the test run. Respawn deletes rows between tests, which is much faster than recreating the database.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Real PostgreSQL.

#### How it works here

`PostgresTestBase` base class.

#### Why it matters here

Lockout counters live in the database, so an in-memory fake would prove nothing.

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW2.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Capturing logger.

#### How it works here

`CapturingLoggerProvider` and its nested `CapturingLogger`.

#### Why it matters here

Proves personal data (email) never reaches logs, by inspecting what was actually logged.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Needs Docker (PostgreSQL container via `PostgresFixture`). Test password and email are throwaway constants.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs`](../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs.md)
- [`src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs`](../../../src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs.md)
- [`src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs`](../../../src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](../Fixtures/PostgresFixture.cs.md)
