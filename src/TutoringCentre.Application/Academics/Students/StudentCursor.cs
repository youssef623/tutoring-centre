using System.Text;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>
/// The student list page cursor: an opaque, URL-safe encoding of the last row's (search name, id) key a page
/// ended on — the same keyset-paging approach as Audit's cursor (Day 32), over a different key shape. Used by
/// the validator to check shape (never trusting the value) and by the read service to resume a page and to
/// build the next one's cursor.
/// </summary>
public static class StudentCursor
{
    private const char Separator = '|';

    public static string Encode(string searchName, Guid id)
    {
        // The id comes first and is fixed-width (32 hex chars), so the separator can never be confused with a
        // character the search name itself might contain.
        var raw = $"{id:N}{Separator}{searchName}";
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static bool TryDecode(string? cursor, out (string SearchName, Guid Id) value)
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
        if (raw.Length < 33 || raw[32] != Separator || !Guid.TryParseExact(raw[..32], "N", out var id))
        {
            return false;
        }

        value = (raw[33..], id);
        return true;
    }
}
