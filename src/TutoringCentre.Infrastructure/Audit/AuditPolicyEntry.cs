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
internal sealed record AuditPolicyEntry(
    AuditEntityType EntityType,
    Func<object, Guid> ResolveCentreId,
    IReadOnlySet<string> AllowedFields,
    IReadOnlySet<string>? MaskedFields = null)
{
    public IReadOnlySet<string> MaskedFields { get; init; } = MaskedFields ?? new HashSet<string>(StringComparer.Ordinal);
}
