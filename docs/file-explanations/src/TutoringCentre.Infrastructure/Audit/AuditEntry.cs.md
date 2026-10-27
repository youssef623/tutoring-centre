# `src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs`

## Purpose

Source summary (intent, not independent execution proof): One append-only row in audit.audit_entries: who changed what, when, and the before/after values. A persistence model, not a Domain entity — it has no behaviour and nothing in the Domain creates or reasons about it; Infrastructure produces it during the command's SaveChanges (Task 31.6). Immutable after construction: rows are never updated once written.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,208 bytes. Source hash: `740220e97b1ad8b7bd9d640d372a08739c855e3602a8ba05332f9e5bd1932fbb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs#L11): `public sealed record AuditEntry : ITenantOwned`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:11](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L11)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:128](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L128)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs:43](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L43)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs:276](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs#L276)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs:282](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs#L282)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs:305](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs#L305)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs:495](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs#L495)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs:568](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs#L568)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs:646](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs#L646)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs:726](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs#L726)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs:884](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs#L884)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs:881](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs#L881)
- [src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs:31](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L31)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs:127](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L127)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:89](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L89)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// One append-only row in audit.audit_entries: who changed what, when, and the before/after values. A persistence
/// model, not a Domain entity — it has no behaviour and nothing in the Domain creates or reasons about it;
/// Infrastructure produces it during the command's SaveChanges (Task 31.6). Immutable after construction: rows
/// are never updated once written.
/// </summary>
public sealed record AuditEntry : ITenantOwned
{
    public required Guid Id { get; init; }

    public required Guid CentreId { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required AuditActorType ActorType { get; init; }

    public Guid? ActorUserId { get; init; }

    public required AuditAction Action { get; init; }

    public required AuditEntityType EntityType { get; init; }

    public required Guid EntityId { get; init; }

    /// <summary>The changes document, as JSON text: <c>{ "&lt;field&gt;": { "before": ..., "after": ... } }</c>. Mapped to jsonb.</summary>
    public required string Changes { get; init; }

    public string? CorrelationId { get; init; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
