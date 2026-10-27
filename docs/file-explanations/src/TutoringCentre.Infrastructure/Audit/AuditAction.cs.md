# `src/TutoringCentre.Infrastructure/Audit/AuditAction.cs`

## Purpose

Source summary (intent, not independent execution proof): What happened to the audited record.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditAction.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 9 lines and 175 bytes. Source hash: `a06714912b245003f37f23d7c3339363c329450221680f7341c5965e6f63188f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditAction.cs#L4): `public enum AuditAction`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs:25](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L25)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs:23](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntry.cs#L23)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:116](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L116)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs:22](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs#L22)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:57](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L57)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:103](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L103)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs:23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L23)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs:20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L20)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:212](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L212)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Audit;

/// <summary>What happened to the audited record.</summary>
public enum AuditAction
{
    Created,
    Updated,
    Deleted,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
