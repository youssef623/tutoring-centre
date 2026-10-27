# `src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs`

## Purpose

Source summary (intent, not independent execution proof): One `AuditPolicy` entry: an audited entity's stable audit name, how to find the centre a change to it belongs to, and the only property names on it that may ever appear in a changes document.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,516 bytes. Source hash: `5cac8a5c88f51d49828fd928168b0988641aff3f44adf993ecb1774c56b2f600`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs#L14): `/// did before action lists existed. A generated class session, say, is recorded only when updated: its creation`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs#L17): `internal sealed record AuditPolicyEntry(`
- [Line 27](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicyEntry.cs#L27): `public bool Records(AuditAction action) => Actions is null || Actions.Contains(action);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:18](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// One <see cref="AuditPolicy"/> entry: an audited entity's stable audit name, how to find the centre a change
/// to it belongs to, and the only property names on it that may ever appear in a changes document.
/// </summary>
/// <param name="MaskedFields">
/// The subset of <paramref name="AllowedFields"/> whose before/after values are masked before being written
/// (Task 38.6) — a phone number, say. Comparison to decide whether the field changed still uses the real
/// value; only what reaches the document is masked. Defaults to none.
/// </param>
/// <param name="Actions">
/// The actions recorded for this entity (Day 52). Null, the default, records every action, exactly as every entry
/// did before action lists existed. A generated class session, say, is recorded only when updated: its creation
/// is a side effect of its rule, which is audited itself.
/// </param>
internal sealed record AuditPolicyEntry(
    AuditEntityType EntityType,
    Func<object, Guid> ResolveCentreId,
    IReadOnlySet<string> AllowedFields,
    IReadOnlySet<string>? MaskedFields = null,
    IReadOnlySet<AuditAction>? Actions = null)
{
    public IReadOnlySet<string> MaskedFields { get; init; } = MaskedFields ?? new HashSet<string>(StringComparer.Ordinal);

    /// <summary>Whether a change of this kind to the entity is written to the audit log.</summary>
    public bool Records(AuditAction action) => Actions is null || Actions.Contains(action);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
