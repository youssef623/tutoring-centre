# src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs

## Purpose
The single EF Core `DbContext`, described as "one transaction boundary across all modules" (line 6). Mapping lives in `IEntityTypeConfiguration` classes in this assembly, so Domain entities carry no persistence code. There are deliberately no `DbSet` properties; repositories will use `Set<T>()`.

## Where it fits
Infrastructure/Persistence.
- **Registered:** by `DependencyInjection.cs:48-53`.
- **Consumer:** `UnitOfWork` (transactions, SaveChanges, ChangeTracker).
- **Future:** repositories and entity configurations.

## Walkthrough
- **Line 10:** `public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)`. A primary constructor passes the options to the base.
- **Lines 12–16:** `OnModelCreating` null-guards, then calls `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)`, which picks up every `IEntityTypeConfiguration<T>` in Infrastructure.

## Concepts used
- **DbContext:** EF Core's unit of work and identity map.
- **Fluent configuration classes:** mapping is separate from entities, which keeps the Domain clean.
- **Primary constructors** (C# 12).

## Data and control flow
Options (Npgsql, snake_case, interceptor) → context → model built from configurations → change tracking → SQL.

## Configuration and environment
None directly. The options come from `DependencyInjection.cs`.

## Gotchas and issues
- **The model is empty.** There are **no configuration classes**, so the model has 0 entity types (verified) and EF logs warning 10632 on model build. `Centre` is not mapped.
- **No migrations exist.**
- It's `public` while the other persistence classes are `internal`. That is probably needed for `dotnet-ef` design-time discovery.

## Related files
- [DependencyInjection.cs](../DependencyInjection.cs.md)
- [UnitOfWork.cs](UnitOfWork.cs.md)
- [Schemas.cs](Schemas.cs.md)
- [Centre.cs](../../TutoringCentre.Domain/Centres/Centre.cs.md)
