namespace TutoringCentre.Application.Conversations.Contracts;

public sealed record ChatCursor(DateTimeOffset At, Guid Id)
{
    public static string Format(DateTimeOffset at, Guid id) => $"{at.UtcTicks}:{id:D}";
    public static bool TryParse(string value, out DateTimeOffset at, out Guid id)
    {
        at = default;
        id = default;
        var parts = value.Split(':');
        if (parts.Length != 2 || !long.TryParse(parts[0], out var ticks) || ticks < 0 || ticks > DateTimeOffset.MaxValue.Ticks
            || !Guid.TryParseExact(parts[1], "D", out id)) return false;
        at = new DateTimeOffset(ticks, TimeSpan.Zero);
        return true;
    }
}
