namespace TutoringCentre.Evaluation.World;

public sealed record Persona(string Key, Guid CentreId, Guid ParentId, Guid UserId, string Email, string TemporaryPassword, IReadOnlyDictionary<string, Guid> Children);
public static class EvalTime { public static readonly DateTimeOffset Now = new(2026, 2, 15, 10, 30, 0, TimeSpan.Zero); }
