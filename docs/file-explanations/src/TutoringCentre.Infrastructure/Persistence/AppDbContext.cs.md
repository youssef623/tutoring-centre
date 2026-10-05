# src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs

## Purpose

The single EF Core database context for the whole application, extended with ASP.NET Core Identity's user, claim, login and token tables (no role tables).

## Where It Fits

Infrastructure/Persistence. Registered by `AddDbContext` in `DependencyInjection.cs`; injected into `UnitOfWork`, `CentreRepository`, `SystemInfoReadService`, `MembershipReadService`, Identity's user store, `MigrationRunner` and some tests.

## Walkthrough

`public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<ApplicationUser, Guid>(options)` (14). The doc comment: one transaction boundary across all modules; mapping in `IEntityTypeConfiguration` classes so Domain carries no persistence code; no `DbSet` properties (repositories use `Set<T>()`); it inherits `IdentityUserContext<TUser, TKey>` (users, claims, logins, tokens) *rather than* `IdentityDbContext` because there are no role tables - a role belongs to a user in a centre (`Membership`), not globally. `OnModelCreating(ModelBuilder builder)` (16-23): null check, `base.OnModelCreating(builder)` first (Identity's own model), then `builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)` - the comment says the project's configurations run afterwards 'so they win'. Those configurations (in `Configurations/Identity/`) rename Identity's tables into the `identity` schema and snake_case names.

## Concepts Used

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Inheriting an Identity context and applying configurations after the base model.

#### How it works here

Lines 14 and 20-22.

#### Why it matters here

The order is significant: the base class builds the default Identity model, then the project's configuration classes override table names, schemas and column details.

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions](../../../../PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

One context, one transaction boundary.

#### How it works here

Class doc comment.

#### Why it matters here

Centres and identity data are saved atomically by the same `SaveChangesAsync`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

A new entity needs an `IEntityTypeConfiguration` class to be mapped as intended. The `identity` schema's tables are renamed by project configurations, not by Identity defaults, so removing them would silently put tables back in the default schema with Identity's names.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](Configurations/Centres/CentreConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs`](Configurations/Identity/ApplicationUserConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs`](Configurations/Identity/MembershipConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../DependencyInjection.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`](UnitOfWork.cs.md)
- [`src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs`](../Identity/ApplicationUser.cs.md)
