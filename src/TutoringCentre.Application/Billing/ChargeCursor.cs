using TutoringCentre.Domain.Common;
using System.Globalization;
using System.Text;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Application.Billing;
public static class ChargeCursor
{
    public static string Encode(BillingMonth month, Guid id) => Convert.ToBase64String(Encoding.UTF8.GetBytes(FormattableString.Invariant($"{month.FirstDay.DayNumber}|{id:N}"))).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    public static bool TryDecode(string? cursor, out (BillingMonth Month, Guid Id) value)
    {
        value = default;
        if (string.IsNullOrEmpty(cursor) || cursor.Length > 128) return false;
        try
        {
            var text = cursor.Replace('-', '+').Replace('_', '/'); text += new string('=', (4 - text.Length % 4) % 4);
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(text)).Split('|');
            if (parts.Length != 2 || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var day) || !Guid.TryParseExact(parts[1], "N", out var id)) return false;
            var date = DateOnly.FromDayNumber(day); if (date.Day != 1 || id == Guid.Empty) return false;
            value = (BillingMonth.Of(date), id); return true;
        }
        catch (FormatException) { return false; }
        catch (ArgumentOutOfRangeException) { return false; }
    }
}
