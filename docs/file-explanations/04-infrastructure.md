# `src/TutoringCentre.Infrastructure`

Folder map generated from the per-file explanations. Part of the [file index](INDEX.md); the teaching overview is [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md). Each row links to the full explanation of that file (purpose, where it fits, walkthrough, concepts, flow, configuration, gotchas, related files). **27 files.**

## `src/TutoringCentre.Infrastructure`

| File | Purpose | Explanation |
| --- | --- | --- |
| `AssemblyMarker.cs` | Assembly handle for architecture tests. | [Explanation](./src/TutoringCentre.Infrastructure/AssemblyMarker.cs.md) |
| `DependencyInjection.cs` | Infrastructure's registration entry point: wires every Application port to its PostgreSQL/EF/system implementation, configures the database, health check and options, and registers ASP.NET Core Identity (core only) with the password and lockout policy. | [Explanation](./src/TutoringCentre.Infrastructure/DependencyInjection.cs.md) |
| `TutoringCentre.Infrastructure.csproj` | Project file for the adapter layer: references Application and Domain and brings in EF Core, the Npgsql provider and the health check. | [Explanation](./src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md) |

## `src/TutoringCentre.Infrastructure/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ApplicationUser.cs` | Infrastructure's persistence model for a staff user, extending ASP.NET Core Identity's `IdentityUser<Guid>`. | [Explanation](./src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs.md) |
| `DevelopmentIdentitySeeder.cs` | Development bootstrapping: creates five staff accounts and their memberships idempotently, using Domain factories. | [Explanation](./src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs.md) |
| `IdentityAuthenticationService.cs` | Implements `IAuthenticationService` with ASP.NET Core Identity's `UserManager`: password check, lockout, no account enumeration. | [Explanation](./src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence`

| File | Purpose | Explanation |
| --- | --- | --- |
| `AppDbContext.cs` | The single EF Core database context for the whole application, extended with ASP.NET Core Identity's user, claim, login and token tables (no role tables). | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs.md) |
| `DatabaseOptions.cs` | Typed settings object holding the database connection string, validated at startup. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs.md) |
| `MigrationRunner.cs` | Extension method that applies pending EF Core migrations from an `IServiceProvider`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs.md) |
| `Schemas.cs` | Constants naming the PostgreSQL schemas used per feature module. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Schemas.cs.md) |
| `UnitOfWork.cs` | Implements the `IUnitOfWork` port with an explicit database transaction on the scoped `AppDbContext`, including PostgreSQL read-only transactions for queries. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CentreConfiguration.cs` | Maps the `Centre` entity to the `platform.centres` table: columns, limits, key, unique index, locale conversion, CHECK constraint and timestamp shadow properties. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ApplicationUserConfiguration.cs` | Maps `ApplicationUser` to `identity.users`, adds the locale CHECK and a unique normalised-email index. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs.md) |
| `IdentityUserClaimConfiguration.cs` | Renames Identity's default `claims` table into `identity.user_claims`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserClaimConfiguration.cs.md) |
| `IdentityUserLoginConfiguration.cs` | Renames Identity's default `logins` table into `identity.user_logins`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserLoginConfiguration.cs.md) |
| `IdentityUserTokenConfiguration.cs` | Renames Identity's default `tokens` table into `identity.user_tokens`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserTokenConfiguration.cs.md) |
| `MembershipConfiguration.cs` | Maps `Membership` to `identity.memberships` with the first foreign keys, composite unique index, converters and CHECK constraints in the schema. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Interceptors`

| File | Purpose | Explanation |
| --- | --- | --- |
| `TimestampInterceptor.cs` | Automatically fills `CreatedAt` and `UpdatedAt` shadow columns just before EF saves. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Migrations`

| File | Purpose | Explanation |
| --- | --- | --- |
| `20261002222404_InitialPlatform.Designer.cs` | EF Core generated target-model snapshot for the migration. | Skipped (generated / lockfile / media), see INDEX |
| `20261002222404_InitialPlatform.cs` | The only migration: creates the `platform` schema, the `centres` table, its primary key, locale CHECK constraint and unique slug index. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.cs.md) |
| `20261004112858_AddIdentityAndMemberships.Designer.cs` | EF Core generated target-model snapshot for the identity migration. | Skipped (generated / lockfile / media), see INDEX |
| `20261004112858_AddIdentityAndMemberships.cs` | The second migration: creates the `identity` schema with `users`, `memberships`, `user_claims`, `user_logins` and `user_tokens`, their keys, foreign keys, indexes and CHECK constraints. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.cs.md) |
| `AppDbContextModelSnapshot.cs` | EF Core generated current-model snapshot (`// <auto-generated />`). | Skipped (generated / lockfile / media), see INDEX |

## `src/TutoringCentre.Infrastructure/ReadServices`

| File | Purpose | Explanation |
| --- | --- | --- |
| `MembershipReadService.cs` | Read-only EF projections for a staff member's profile, active memberships and session state. | [Explanation](./src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs.md) |
| `SystemInfoReadService.cs` | Read-side implementation that reports applied and pending EF migrations as plain data. | [Explanation](./src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs.md) |

## `src/TutoringCentre.Infrastructure/Repositories`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CentreRepository.cs` | EF Core implementation of `ICentreRepository`. | [Explanation](./src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs.md) |

## `src/TutoringCentre.Infrastructure/Time`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SystemClock.cs` | Real implementation of `IClock` based on `TimeProvider` and the OS time-zone database. | [Explanation](./src/TutoringCentre.Infrastructure/Time/SystemClock.cs.md) |
