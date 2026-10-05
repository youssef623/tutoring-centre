# src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs

## Purpose

Development bootstrapping: creates five staff accounts and their memberships idempotently, using Domain factories.

## Where It Fits

Infrastructure/Identity. Called by `SeedCommand.RunAsync` (after the centres). Depends on `UserManager`, `AppDbContext`, `IConfiguration`, `Membership.Create`, `Centre`. Not a use case; never runs at normal startup or during OpenAPI generation.

## Walkthrough

`SeedAsync(CancellationToken)` (33-68): requires configuration key `Seed:Password` (blank -> `Error.Rule("seed.password_missing")`); loads centre ids by slug; for each seed user: `GetOrCreateUserAsync` (70-89; `FindByEmailAsync`, else `userManager.CreateAsync(user, password)` with `EmailConfirmed = true`), then for each membership `EnsureMembershipAsync` (91-107; skipped if (user, centre) already exists; otherwise `Membership.Create(...).Value`, and `Deactivate()` when the spec says inactive); finally one `db.SaveChangesAsync`. Missing centre -> `seed.centre_missing`.
Seed set (20-31): `owner@nile.test` (Owner, nile-centre, ar), `owner@maadi.test` (Owner, maadi-hub, en), `teacher@both.test` (Teacher in both), `secretary@nile.test` (Secretary, nile-centre), `inactive@nile.test` (Secretary, **inactive** membership).

## Concepts Used

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Seeding via `UserManager`.

#### How it works here

Lines 70-89.

#### Why it matters here

Passwords are hashed by Identity, never written by hand.

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../../../../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

`Seed:Password` from configuration.

#### How it works here

Lines 18, 35.

#### Why it matters here

No password in the repository; the developer supplies it via user-secrets.

### Entities, encapsulation and factory methods

#### What it means

An *entity* has an identity that stays the same while its data changes. *Encapsulation* keeps its state behind rules (private setters, private constructors). A *factory method* is the only public way to build one, so an invalid instance cannot exist.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW2.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

Domain factories.

#### How it works here

`Membership.Create`, `Deactivate`.

#### Why it matters here

Seeded data obeys the same rules as app data.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Configuration key `Seed:Password` (secret; user-secrets locally, `Seed__Password` in CI). Centre slugs `nile-centre`, `maadi-hub` must already exist.

## Gotchas and Issues

All five accounts share one password (the one configured). `SaveChanges` is called once at the end, but `UserManager.CreateAsync` has already persisted users individually, so a failure mid-way can leave users without memberships (re-running repairs it).

## Related Files

- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../TutoringCentre.Api/Cli/SeedCommand.cs.md)
- [`src/TutoringCentre.Domain/Identity/Membership.cs`](../../TutoringCentre.Domain/Identity/Membership.cs.md)
- [`tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs`](../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs.md)
- [`README.md`](../../../README.md.md)
