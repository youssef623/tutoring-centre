# `src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs`

## Purpose

The single context combines Identity user storage and ordinary domain mappings with the platform Data Protection key context. It reads the actor's centre live, applies assembly configurations, allows a test-model extension, then installs tenant filters last. Only the required DataProtectionKeys DbSet is exposed explicitly; adapters use Set<T> for other types.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 3,326 bytes. Source hash: `287d32c65a1394be9449b35831579c03a63af5ca288db8fbb91a445bfd2bd612`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;`
- Line 2: `using Microsoft.AspNetCore.Identity.EntityFrameworkCore;`
- Line 3: `using Microsoft.EntityFrameworkCore;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence.Conventions;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs#L21): `public class AppDbContext : IdentityUserContext<ApplicationUser, Guid>, IDataProtectionKeyContext`
- [Line 25](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs#L25): `public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/AppDbContextFactory.cs:15](../../../../../src/TutoringCentre.Api/Cli/AppDbContextFactory.cs#L15)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:206](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L206)
- [src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs:19](../../../../../src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs#L19)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:103](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L103)
- [src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs:16](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L16)
- [src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs:16](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs#L16)
- [src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs:38](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L38)
- [src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs:31](../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L31)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs#L14)
- 42 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence.Conventions;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The single EF Core context: one transaction boundary across all modules. Mapping lives in
/// IEntityTypeConfiguration classes in this assembly, so Domain entities carry no persistence code.
/// Deliberately no DbSet properties beyond <see cref="DataProtectionKeys"/> — repositories use Set&lt;T&gt;() for
/// everything else; the Data Protection key ring (Task 34.6) is the one documented exception, because
/// <see cref="IDataProtectionKeyContext"/> requires exactly that property.
/// Inherits <see cref="IdentityUserContext{TUser,TKey}"/> (users, claims, logins, tokens) rather than
/// IdentityDbContext: there are no role tables — a role belongs to a user in a centre (Membership), not globally.
/// Not sealed only so the test-only probe context (Task 20.3) can extend it via <see cref="ExtendModel"/>; nothing
/// else about it is open for override.
/// </summary>
public class AppDbContext : IdentityUserContext<ApplicationUser, Guid>, IDataProtectionKeyContext
{
    private readonly ICurrentActor _currentActor;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(currentActor);
        _currentActor = currentActor;
    }

    /// <summary>
    /// The acting actor's centre, read live from <see cref="ICurrentActor"/> on every access rather than cached at
    /// construction: a request can build this context (e.g. through DI) before the actor is set on it — login sets
    /// the actor after the scope, and therefore after the context, already exists. Paths that build a context with
    /// no request (migrations, design-time tooling) supply an anonymous actor, so this is null there.
    /// </summary>
    public Guid? CurrentCentreId => _currentActor.Actor.CentreId;

    /// <summary>The Data Protection key ring (Task 34.6) — platform-level, not tenant-owned, no RLS.</summary>
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected sealed override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Identity's own user/claims/logins/tokens model runs first; the project's configurations below are applied after, so they win.
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Test-only extra mappings (Task 20.3) go here, strictly before the tenant filter convention below, so the
        // convention sees every mapped entity — including a test probe — and nothing can be configured after it.
        ExtendModel(builder);

        TenantQueryFilterConvention.Apply(builder, this);
    }

    /// <summary>Extension point for the test-only probe context (Task 20.3) to map its extra entity. No production override.</summary>
    protected virtual void ExtendModel(ModelBuilder builder)
    {
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
