using System.Globalization;
using System.Text;

namespace TutoringCentre.Application.Audit;

/// <summary>
/// The audit page cursor: an opaque, URL-safe encoding of the last row's (occurredAt, id) pair a page ended
/// on. Used by the validator to check shape (never trusting the value) and by the read service to resume a
/// page and to build the next one's cursor.
/// </summary>
public static class AuditCursor
{
    private const char Separator = '|';

    public static string Encode(DateTimeOffset occurredAt, Guid id)
    {
        var raw = $"{occurredAt.UtcTicks.ToString(CultureInfo.InvariantCulture)}{Separator}{id:N}";
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static bool TryDecode(string? cursor, out (DateTimeOffset OccurredAt, Guid Id) value)
    {
        value = default;
        if (string.IsNullOrEmpty(cursor))
        {
            return false;
        }

        var padded = cursor.Replace('-', '+').Replace('_', '/');
        var remainder = padded.Length % 4;
        if (remainder != 0)
        {
            padded += new string('=', 4 - remainder);
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(padded);
        }
        catch (FormatException)
        {
            return false;
        }

        var raw = Encoding.UTF8.GetString(bytes);
        var parts = raw.Split(Separator);
        if (parts.Length != 2
            || !long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
            || !Guid.TryParseExact(parts[1], "N", out var id))
        {
            return false;
        }

        value = (new DateTimeOffset(ticks, TimeSpan.Zero), id);
        return true;
    }
}
