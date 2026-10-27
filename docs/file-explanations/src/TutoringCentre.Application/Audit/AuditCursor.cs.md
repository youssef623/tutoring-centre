# `src/TutoringCentre.Application/Audit/AuditCursor.cs`

## Purpose

Encodes the descending audit keyset position as timestamp ticks plus event ID, transported as an opaque Base64 token. Decoding checks token structure and numeric/GUID parsing, but DateTimeOffset construction can throw for out-of-range ticks; callers should not assume every malformed token returns false.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Audit/AuditCursor.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 59 lines and 1,833 bytes. Source hash: `73c57d09ba6c3bd14055802b93ed53218a16006884e4aad7ee768907581aca92`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Text;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../src/TutoringCentre.Application/Audit/AuditCursor.cs#L11): `public static class AuditCursor`
- [Line 15](../../../../../src/TutoringCentre.Application/Audit/AuditCursor.cs#L15): `public static string Encode(DateTimeOffset occurredAt, Guid id)`
- [Line 22](../../../../../src/TutoringCentre.Application/Audit/AuditCursor.cs#L22): `public static bool TryDecode(string? cursor, out (DateTimeOffset OccurredAt, Guid Id) value)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:36](../../../../onboarding-work/extend_backend_purposes.py#L36)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs:34](../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs#L34)
- [src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs:58](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L58)
- [tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs:115](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L115)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
