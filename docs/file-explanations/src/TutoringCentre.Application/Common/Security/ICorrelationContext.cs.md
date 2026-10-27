# `src/TutoringCentre.Application/Common/Security/ICorrelationContext.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-only access to the current scope's correlation ID (HTTP request or job). Null when none was set.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/ICorrelationContext.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 256 bytes. Source hash: `482646b09e98067648b727515719500df01ae59253f050ad9db7a712bcd31465`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../src/TutoringCentre.Application/Common/Security/ICorrelationContext.cs#L4): `public interface ICorrelationContext`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Common/Security/CorrelationContext.cs:7](../../../../../../src/TutoringCentre.Application/Common/Security/CorrelationContext.cs#L7)
- [src/TutoringCentre.Application/DependencyInjection.cs:20](../../../../../../src/TutoringCentre.Application/DependencyInjection.cs#L20)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:20](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Security;

/// <summary>Read-only access to the current scope's correlation ID (HTTP request or job). Null when none was set.</summary>
public interface ICorrelationContext
{
    string? CorrelationId { get; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
