# `src/TutoringCentre.Domain/Common/ITenantOwned.cs`

## Purpose

Source summary (intent, not independent execution proof): Marks an entity that belongs to exactly one centre (tenant). Infrastructure applies tenant filtering to every implementer automatically (Month 2). Centre itself does not implement it: it is the tenant.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/ITenantOwned.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 341 bytes. Source hash: `cef5df4f4527562ed57e828ba239aba7db42f63261604dded1aa1bce0ba8e95a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Domain/Common/ITenantOwned.cs#L7): `public interface ITenantOwned`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Domain/Academics/Enrollment.cs:14](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L14)
- [src/TutoringCentre.Domain/Academics/Group.cs:13](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L13)
- [src/TutoringCentre.Domain/Academics/Parent.cs:10](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L10)
- [src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs:14](../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L14)
- [src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs:13](../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L13)
- [src/TutoringCentre.Domain/Academics/Student.cs:12](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L12)
- [src/TutoringCentre.Domain/Academics/StudentParentLink.cs:10](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L10)
- [src/TutoringCentre.Domain/Academics/Subject.cs:7](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L7)
- [src/TutoringCentre.Domain/Academics/Teacher.cs:14](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L14)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs:11](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs#L11)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/TenantOwnedConfiguration.cs:18](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/TenantOwnedConfiguration.cs#L18)
- [src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs:21](../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs#L21)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs:53](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L53)
- [tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs:25](../../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L25)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbe.cs:9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbe.cs#L9)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChild.cs:9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChild.cs#L9)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Common;

/// <summary>
/// Marks an entity that belongs to exactly one centre (tenant). Infrastructure applies tenant filtering
/// to every implementer automatically (Month 2). Centre itself does not implement it: it is the tenant.
/// </summary>
public interface ITenantOwned
{
    Guid CentreId { get; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
