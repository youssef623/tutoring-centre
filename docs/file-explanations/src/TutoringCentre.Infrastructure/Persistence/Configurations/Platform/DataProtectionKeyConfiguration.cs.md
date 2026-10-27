# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the ASP.NET Core Data Protection key ring (Task 34.6) to platform.data_protection_keys — platform-level, not tenant-owned, so it carries no centre column and no row-level security, and is the one place AppDbContext exposes a DbSet property (`AppDbContext.DataProtectionKeys`) rather than Set&lt;T&gt;(): the IDataProtectionKeyContext interface the Data Protection stack persists through requires exactly that.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,061 bytes. Source hash: `2ac4135acfd23219e291628e2919de32eca78adf4c2d8e341b03f3d250fb0eaa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs#L11): `/// <c>IDataProtectionKeyContext</c> interface the Data Protection stack persists through requires exactly that.`
- [Line 13](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs#L13): `internal sealed class DataProtectionKeyConfiguration : IEntityTypeConfiguration<DataProtectionKey>`
- [Line 15](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs#L15): `public void Configure(EntityTypeBuilder<DataProtectionKey> builder)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:36](../../../../../../../db/bootstrap-roles.sh#L36)
- [docs/onboarding-work/assemble.py:14](../../../../../../onboarding-work/assemble.py#L14)
- [docs/onboarding-work/enrich_frontend.py:31](../../../../../../onboarding-work/enrich_frontend.py#L31)
- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../../../onboarding-work/extend_backend_purposes.py#L8)
- [docs/onboarding-work/file_guides.py:40](../../../../../../onboarding-work/file_guides.py#L40)
- [docs/onboarding-work/inspect-tests.py:25](../../../../../../onboarding-work/inspect-tests.py#L25)
- [docs/onboarding-work/inventory.py:1](../../../../../../onboarding-work/inventory.py#L1)
- [docs/onboarding-work/read-generated-models.py:45](../../../../../../onboarding-work/read-generated-models.py#L45)
- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [frontend/e2e/auth.spec.ts:7](../../../../../../../frontend/e2e/auth.spec.ts#L7)
- [frontend/e2e/enrollment.spec.ts:7](../../../../../../../frontend/e2e/enrollment.spec.ts#L7)
- [frontend/e2e/first-login.spec.ts:7](../../../../../../../frontend/e2e/first-login.spec.ts#L7)
- [frontend/e2e/full-centre-setup.spec.ts:7](../../../../../../../frontend/e2e/full-centre-setup.spec.ts#L7)
- [frontend/e2e/schedule.spec.ts:7](../../../../../../../frontend/e2e/schedule.spec.ts#L7)
- [frontend/e2e/staff-access.spec.ts:7](../../../../../../../frontend/e2e/staff-access.spec.ts#L7)
- [frontend/e2e/staff-audit.spec.ts:7](../../../../../../../frontend/e2e/staff-audit.spec.ts#L7)
- [frontend/e2e/students.spec.ts:7](../../../../../../../frontend/e2e/students.spec.ts#L7)
- [frontend/e2e/subjects.spec.ts:7](../../../../../../../frontend/e2e/subjects.spec.ts#L7)
- [frontend/e2e/teachers-groups.spec.ts:7](../../../../../../../frontend/e2e/teachers-groups.spec.ts#L7)
- [frontend/src/api/apiFetch.test.ts:9](../../../../../../../frontend/src/api/apiFetch.test.ts#L9)
- 233 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Platform;

/// <summary>
/// Maps the ASP.NET Core Data Protection key ring (Task 34.6) to platform.data_protection_keys — platform-level,
/// not tenant-owned, so it carries no centre column and no row-level security, and is the one place AppDbContext
/// exposes a DbSet property (<see cref="AppDbContext.DataProtectionKeys"/>) rather than Set&lt;T&gt;(): the
/// <c>IDataProtectionKeyContext</c> interface the Data Protection stack persists through requires exactly that.
/// </summary>
internal sealed class DataProtectionKeyConfiguration : IEntityTypeConfiguration<DataProtectionKey>
{
    public void Configure(EntityTypeBuilder<DataProtectionKey> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("data_protection_keys", Schemas.Platform);
        builder.HasKey(key => key.Id);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
