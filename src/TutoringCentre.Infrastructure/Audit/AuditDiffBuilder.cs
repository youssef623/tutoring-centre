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

    private static JsonValue? ToJsonValue(object? value) => value switch
    {
        null => null,
        string stringValue => JsonValue.Create(stringValue),
        bool boolValue => JsonValue.Create(boolValue),
        Guid guidValue => JsonValue.Create(guidValue.ToString()),
        Enum enumValue => JsonValue.Create(enumValue.ToString().ToLowerInvariant()),

        // ISO and culture-invariant (Day 52's schedule rules): ToString() alone would follow the server's culture.
        DateOnly dateValue => JsonValue.Create(dateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
        TimeOnly timeValue => JsonValue.Create(timeValue.ToString("HH:mm", CultureInfo.InvariantCulture)),
        sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal =>
            JsonValue.Create(Convert.ToDouble(value, CultureInfo.InvariantCulture)),
        _ => JsonValue.Create(value.ToString()),
    };

    // Entity property names are PascalCase (e.g. "DefaultLocale"); the contract's field names are camelCase.
    private static string ToCamelCase(string name) => $"{char.ToLowerInvariant(name[0])}{name[1..]}";
}
