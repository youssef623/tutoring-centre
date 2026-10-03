# tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs

## Purpose

Proves a brand-new database built only from the migrations has the expected objects.

## Where It Fits

Infrastructure.Tests/Persistence. Relies on `PostgresFixture` having run `ApplyMigrationsAsync`.

## Walkthrough

Four tests with raw SQL: `to_regclass('platform.centres') is not null`; `pg_indexes` row for `ux_centres_slug` starts with `CREATE UNIQUE INDEX` and mentions `slug`; one row in `information_schema.check_constraints` for `ck_centres_default_locale`; `to_regclass('platform.__ef_migrations_history')` exists (history table is in the `platform` schema). Not tested: column types/nullability, the `Down` migration.

## Concepts Used

### Database migrations

#### What it means

A migration is versioned code describing one schema change with an `Up` (apply) and `Down` (revert). EF records applied migrations in a history table and keeps a *model snapshot* of the last known model; the next `migrations add` diffs the current model against it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Verifying generated schema.

#### How it works here

All four.

#### Why it matters here

Catches drift between model and migration SQL.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](../Fixtures/PostgresFixture.cs.md)
