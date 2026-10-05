# src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs

## Purpose

Maps `ApplicationUser` to `identity.users`, adds the locale CHECK and a unique normalised-email index.

## Where It Fits

Infrastructure/Persistence/Configurations/Identity, `internal`. Applied by `AppDbContext.OnModelCreating` after Identity's own base configuration, so it wins (doc comment).

## Walkthrough

`ToTable("users", Schemas.Identity, t => t.HasCheckConstraint("ck_users_preferred_locale", "preferred_locale IN ('ar','en')"))` (17-20); `DisplayName` required max 120; `PreferredLocale` required max 2; `MustChangePassword` required; `HasIndex(NormalizedEmail).IsUnique().HasDatabaseName("ux_users_normalized_email")` (26).

## Concepts Used

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Fluent mapping over Identity's model.

#### How it works here

`Configure`.

#### Why it matters here

Renames and constrains Identity's default table.

### Two-tier validation

#### What it means

Validation asks whether input is acceptable. Cheap *shape* checks (required, length) can run first without touching business rules or the database; *business invariants* (slug format, time zone must exist) belong to the domain; *database constraints* are a last safety net against rows written by anything else.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#65-validation-two-tiers](../../../../../../PROJECT_OVERVIEW2.md#65-validation-two-tiers).)

#### Where it appears in this file

Database constraints.

#### How it works here

CHECK and unique index.

#### Why it matters here

Duplicate emails and bad locales are rejected by PostgreSQL.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Identity also creates its own unique `UserNameIndex` on normalised user name (visible in the migration).

## Related Files

- [`src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs`](../../../Identity/ApplicationUser.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs`](../../AppDbContext.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.cs`](../../Migrations/20261004112858_AddIdentityAndMemberships.cs.md)
