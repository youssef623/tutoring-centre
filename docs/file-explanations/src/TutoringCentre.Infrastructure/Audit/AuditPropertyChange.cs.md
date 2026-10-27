# `src/TutoringCentre.Infrastructure/Audit/AuditPropertyChange.cs`

## Purpose

Source summary (intent, not independent execution proof): One tracked property's plain-value shape, as `AuditDiffBuilder` needs it — no EF type involved, so the builder can be tested without a change tracker or a database.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPropertyChange.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 383 bytes. Source hash: `94fbd87d21bc423c837e09c574922efce8b066f3ec7d77bb1fcf4d1aa8f265d7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPropertyChange.cs#L7): `internal sealed record AuditPropertyChange(string Name, object? OriginalValue, object? CurrentValue, bool IsModified);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs:27](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L27)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:115](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L115)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// One tracked property's plain-value shape, as <see cref="AuditDiffBuilder"/> needs it — no EF type involved,
/// so the builder can be tested without a change tracker or a database.
/// </summary>
internal sealed record AuditPropertyChange(string Name, object? OriginalValue, object? CurrentValue, bool IsModified);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
