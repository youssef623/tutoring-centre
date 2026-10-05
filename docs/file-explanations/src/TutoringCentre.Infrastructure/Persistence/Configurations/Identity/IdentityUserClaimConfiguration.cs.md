# src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserClaimConfiguration.cs

## Purpose

Renames Identity's default `claims` table into `identity.user_claims`.

## Where It Fits

Infrastructure/Persistence/Configurations/Identity, `internal`. Applied by `AppDbContext.OnModelCreating`.

## Walkthrough

`IEntityTypeConfiguration<IdentityUserClaim<Guid>>` with a single call `builder.ToTable("user_claims", Schemas.Identity)`. Without it Identity's default would use `AspNetUserClaims` in the default schema; the snake_case naming convention and the project's schema-per-module rule need an explicit name.

## Concepts Used

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Table renaming.

#### How it works here

`ToTable`.

#### Why it matters here

Consistent naming with the rest of the schema.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The `user_claims` table exists because Identity's model includes it; nothing in the repo uses external logins, claims or tokens yet.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs`](ApplicationUserConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Schemas.cs`](../../Schemas.cs.md)
