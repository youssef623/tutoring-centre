# `src/TutoringCentre.Domain/Common/Entity.cs`

## Purpose

Source summary (intent, not independent execution proof): Base class for entities: identity is a UUIDv7 assigned in the application at construction.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/Entity.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 396 bytes. Source hash: `b2b8b18298466ef95e14ff72e00729cb6f493187ff3ad8ba6c4ee6d2c5f85ffd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../src/TutoringCentre.Domain/Common/Entity.cs#L3): `/// <summary>Base class for entities: identity is a UUIDv7 assigned in the application at construction.</summary>`
- [Line 4](../../../../../src/TutoringCentre.Domain/Common/Entity.cs#L4): `public abstract class Entity`
- [Line 6](../../../../../src/TutoringCentre.Domain/Common/Entity.cs#L6): `protected Entity() => Id = Guid.CreateVersion7();`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:14](../../../../onboarding-work/extend_backend_purposes.py#L14)
- [docs/onboarding-work/read-generated-models.py:9](../../../../onboarding-work/read-generated-models.py#L9)
- [frontend/e2e/staff-audit.spec.ts:63](../../../../../frontend/e2e/staff-audit.spec.ts#L63)
- [frontend/src/routes/_authenticated/audit.test.tsx:190](../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L190)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs:22](../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs#L22)
- [src/TutoringCentre.Domain/Academics/Enrollment.cs:14](../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L14)
- [src/TutoringCentre.Domain/Academics/Group.cs:13](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L13)
- [src/TutoringCentre.Domain/Academics/Parent.cs:10](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L10)
- [src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs:14](../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L14)
- [src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs:13](../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L13)
- [src/TutoringCentre.Domain/Academics/Student.cs:12](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L12)
- [src/TutoringCentre.Domain/Academics/StudentParentLink.cs:10](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L10)
- [src/TutoringCentre.Domain/Academics/Subject.cs:7](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L7)
- [src/TutoringCentre.Domain/Academics/Teacher.cs:14](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L14)
- [src/TutoringCentre.Domain/Centres/Centre.cs:7](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L7)
- [src/TutoringCentre.Domain/Identity/Membership.cs:10](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L10)
- [src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs:38](../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs#L38)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:88](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L88)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs:47](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L47)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.Designer.cs:28](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.Designer.cs#L28)
- 15 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Common;

/// <summary>Base class for entities: identity is a UUIDv7 assigned in the application at construction.</summary>
public abstract class Entity
{
    protected Entity() => Id = Guid.CreateVersion7();

    /// <summary>Private setter: only the entity itself (and EF Core when loading, Day 8) can set it.</summary>
    public Guid Id { get; private set; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
