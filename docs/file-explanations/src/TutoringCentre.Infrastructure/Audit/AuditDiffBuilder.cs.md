# `src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs`

## Purpose

The pure diff builder limits output to allowlisted fields and records created, updated, or deleted before/after values. Updated fields require both modification state and real inequality. Phone masking occurs after comparison, avoiding missed changes when masked displays coincide. A null diff means no audit row, and JSON primitive representation is centralized here.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 110 lines and 5,244 bytes. Source hash: `e3b912d7874d4e4156ef6da12b2773bad1e637015f1ea64952f2076be03fd34d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Text.Json.Nodes;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L14): `internal static class AuditDiffBuilder`
- [Line 24](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L24): `public static string? Build(`
- [Line 69](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L69): `private static JsonObject BuildEntry(object? before, object? after) => new()`
- [Line 77](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L77): `private static object? Represent(string field, IReadOnlySet<string> maskedFields, object? value)`
- [Line 92](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L92): `private static JsonValue? ToJsonValue(object? value) => value switch`
- [Line 109](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L109): `private static string ToCamelCase(string name) => $"{char.ToLowerInvariant(name[0])}{name[1..]}";`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:122](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L122)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs:23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L23)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs:20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using System.Text.Json.Nodes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// Turns an action, the entity's allowed fields (Task 31.4's <see cref="AuditPolicyEntry.AllowedFields"/>) and
/// its tracked property changes into the audit entry's "changes" document (the Day 31 contract). Takes and
/// returns plain values only — no EF type appears here — so it can be tested exhaustively without a database.
/// The allow-list is applied here, at the single point where values become audit data: a property not in
/// <paramref name="allowedFields"/> can never reach the output, however it changed.
/// </summary>
internal static class AuditDiffBuilder
{
    /// <returns>The changes document as JSON text, or null when there is nothing to record (an update where no
    /// allowed field actually changed) — null means no audit row is written at all, not an empty one.</returns>
    /// <param name="maskedFields">
    /// Task 38.6: the subset of <paramref name="allowedFields"/> whose before/after values are masked before
    /// being written. Whether a field changed is still decided from the real values (<paramref
    /// name="propertyChanges"/>), before masking is applied to what is written — two different phone numbers
    /// that happen to share their last four digits must still produce an entry.
    /// </param>
    public static string? Build(
        AuditAction action,
        IReadOnlySet<string> allowedFields,
        IReadOnlyList<AuditPropertyChange> propertyChanges,
        IReadOnlySet<string>? maskedFields = null)
    {
        ArgumentNullException.ThrowIfNull(allowedFields);
        ArgumentNullException.ThrowIfNull(propertyChanges);

        var masked = maskedFields ?? new HashSet<string>(StringComparer.Ordinal);
        var changesByName = propertyChanges.ToDictionary(change => change.Name, StringComparer.Ordinal);
        var document = new JsonObject();

        foreach (var field in allowedFields)
        {
            changesByName.TryGetValue(field, out var change);

            switch (action)
            {
                case AuditAction.Created:
                    document[ToCamelCase(field)] = BuildEntry(before: null, after: Represent(field, masked, change?.CurrentValue));
                    break;

                case AuditAction.Deleted:
                    document[ToCamelCase(field)] = BuildEntry(before: Represent(field, masked, change?.OriginalValue), after: null);
                    break;

                case AuditAction.Updated:
                    if (change is { IsModified: true } modified && !Equals(modified.OriginalValue, modified.CurrentValue))
                    {
                        document[ToCamelCase(field)] = BuildEntry(
                            Represent(field, masked, modified.OriginalValue),
                            Represent(field, masked, modified.CurrentValue));
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, "Unsupported audit action.");
            }
        }

        return document.Count == 0 ? null : document.ToJsonString();
    }

    private static JsonObject BuildEntry(object? before, object? after) => new()
    {
        ["before"] = ToJsonValue(before),
        ["after"] = ToJsonValue(after),
    };

    /// <summary>Masking applies only to what is written — the comparison in <see cref="Build"/> above already
    /// ran on the real value by the time this is called.</summary>
    private static object? Represent(string field, IReadOnlySet<string> maskedFields, object? value)
    {
        if (!maskedFields.Contains(field))
        {
            return value;
        }

        return value switch
        {
            null => null,
            PhoneNumber phone => phone.Masked,
            _ => throw new InvalidOperationException($"Masked field '{field}' must hold a PhoneNumber or null, not {value.GetType().Name}."),
        };
    }
```

The excerpt stops at line 90; use the original file for the remaining 20 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
