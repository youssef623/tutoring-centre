namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// One tracked property's plain-value shape, as <see cref="AuditDiffBuilder"/> needs it — no EF type involved,
/// so the builder can be tested without a change tracker or a database.
/// </summary>
internal sealed record AuditPropertyChange(string Name, object? OriginalValue, object? CurrentValue, bool IsModified);
