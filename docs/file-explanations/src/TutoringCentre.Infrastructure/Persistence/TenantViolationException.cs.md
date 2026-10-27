# `src/TutoringCentre.Infrastructure/Persistence/TenantViolationException.cs`

## Purpose

Signals a tenant write-guard rejection and identifies the CLR entity type for internal diagnostics. It is intentionally not a business Result: uncaught guard violations reach the generic HTTP exception path, because they indicate a programming or isolation failure.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/TenantViolationException.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 505 bytes. Source hash: `9f333e1e7da58af85cd17927b39c17faee8e4cf4aa5072413f03e4de0457fb31`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Infrastructure/Persistence/TenantViolationException.cs#L7): `public sealed class TenantViolationException : Exception`
- [Line 9](../../../../../src/TutoringCentre.Infrastructure/Persistence/TenantViolationException.cs#L9): `public TenantViolationException(Type entityType)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:28](../../../../onboarding-work/extend_backend_purposes.py#L28)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs:47](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L47)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs:138](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L138)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs:134](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L134)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs:31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L31)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Thrown by <see cref="Interceptors.TenantWriteGuardInterceptor"/> when a save would cross a tenant boundary.
/// Nothing below the dispatcher catches it; the global exception handler turns it into a generic 500.
/// </summary>
public sealed class TenantViolationException : Exception
{
    public TenantViolationException(Type entityType)
        : base($"Save rejected: '{entityType.Name}' crosses a tenant boundary.")
    {
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
