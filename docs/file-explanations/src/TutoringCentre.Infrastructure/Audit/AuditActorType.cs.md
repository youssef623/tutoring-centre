# `src/TutoringCentre.Infrastructure/Audit/AuditActorType.cs`

## Purpose

Source summary (intent, not independent execution proof): Who made the change an audit entry records.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditActorType.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 169 bytes. Source hash: `1b962c1df34adbbc5590dfd7e9b16814ed5b94e4d80993790a8094f78e22383a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditActorType.cs#L4): `public enum AuditActorType`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs:19](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs#L19)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:42](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L42)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:73](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L73)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Audit;

/// <summary>Who made the change an audit entry records.</summary>
public enum AuditActorType
{
    Staff,
    System,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
