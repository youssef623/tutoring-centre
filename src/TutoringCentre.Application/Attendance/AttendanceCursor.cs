using System.Globalization;
using System.Text;

namespace TutoringCentre.Application.Attendance;

public static class AttendanceCursor
{
    public static string Encode(DateOnly date, DateTimeOffset startsAt, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(FormattableString.Invariant($"{date.DayNumber}|{startsAt.UtcTicks}|{id:N}")))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static bool TryDecode(string? cursor, out (DateOnly Date, DateTimeOffset StartsAt, Guid Id) value)
    {
        value = default;
        if (string.IsNullOrEmpty(cursor) || cursor.Length > 256) { return false; }
        try
        {
            var padded = cursor.Replace('-', '+').Replace('_', '/');
            padded += new string('=', (4 - padded.Length % 4) % 4);
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(padded)).Split('|');
            if (parts.Length != 3
                || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var day)
                || !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var ticks)
                || !Guid.TryParseExact(parts[2], "N", out var id)) { return false; }
            value = (DateOnly.FromDayNumber(day), new DateTimeOffset(ticks, TimeSpan.Zero), id);
            return true;
        }
        catch (FormatException) { return false; }
        catch (ArgumentOutOfRangeException) { return false; }
    }
}
