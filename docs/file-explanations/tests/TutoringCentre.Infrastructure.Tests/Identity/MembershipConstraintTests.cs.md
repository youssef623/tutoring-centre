# tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs

## Purpose

Proves that PostgreSQL itself enforces the identity schema: exactly five identity tables, unique (centre, user) membership, foreign key to centres, and no deleting a centre that has members.

## Where It Fits

Infrastructure.Tests/Identity; uses the real migrated schema from `PostgresFixture`.

## Walkthrough

- `Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables` (17-34): queries `information_schema.tables where table_schema = 'identity'` and asserts the list equals `memberships, user_claims, user_logins, user_tokens, users` - so Identity role tables (`roles`, `user_roles`, `role_claims`) are absent.
- `SaveChanges_SecondMembershipForSameCentreAndUser_ViolatesUniqueIndex` (36-52): two memberships for the same user and centre (different roles); second `SaveChangesAsync` throws `DbUpdateException` whose inner `PostgresException.SqlState` is `23505` (unique violation).
- `SaveChanges_MembershipForNonExistentCentre_ViolatesForeignKey` (54-66): random centre id -> `23503` (foreign key violation).
- `Delete_CentreWithAMembership_IsRejected` (68-85): raw `delete from platform.centres where id = @id` -> `PostgresException` `23503` (the FK is not cascading).
Helpers `CreateCentreAsync` (86-91; sends `CreateCentreCommand` as `SystemActor(null)`) and `CreateUserAsync` (93-107; adds an `ApplicationUser` directly through `AppDbContext`).

## Concepts Used

### Concurrency, TOCTOU and unique indexes

#### What it means

TOCTOU (time of check to time of use): code checks a condition then acts, but another request can change the condition in between. Only the database can make "no two rows share a slug" true, via a unique index; application checks just produce friendlier errors in the common case.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#610-concurrency-the-slug-race-and-why-the-unique-index-is-the-real-guarantee](../../../../PROJECT_OVERVIEW2.md#610-concurrency-the-slug-race-and-why-the-unique-index-is-the-real-guarantee).)

#### Where it appears in this file

Constraints as the real guarantee.

#### How it works here

`SaveChanges_SecondMembershipForSameCentreAndUser_ViolatesUniqueIndex`.

#### Why it matters here

Same idea as the slug unique index: the database, not application code, makes duplicates impossible.

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Schema shape.

#### How it works here

`Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables`.

#### Why it matters here

Documents the decision to use Identity's core user tables without role tables (roles live on `Membership`).

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

SQLSTATE codes.

#### How it works here

The `SqlState` assertions in the three constraint tests.

#### Why it matters here

`DbUpdateException` wraps the Npgsql exception; `23505`/`23503` are PostgreSQL's standard codes.

### Database migrations

#### What it means

A migration is versioned code describing one schema change with an `Up` (apply) and `Down` (revert). EF records applied migrations in a history table and keeps a *model snapshot* of the last known model; the next `migrations add` diffs the current model against it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Applied migration shape.

#### How it works here

`Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables`.

#### Why it matters here

Tests the *result* of running the migration on an empty database.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Needs Docker.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.cs.md)
- [`src/TutoringCentre.Domain/Identity/Membership.cs`](../../../src/TutoringCentre.Domain/Identity/Membership.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](../Fixtures/PostgresFixture.cs.md)
