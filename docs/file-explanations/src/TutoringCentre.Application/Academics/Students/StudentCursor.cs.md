# `src/TutoringCentre.Application/Academics/Students/StudentCursor.cs`

## Purpose

Source summary (intent, not independent execution proof): The student list page cursor: an opaque, URL-safe encoding of the last row's (search name, id) key a page ended on — the same keyset-paging approach as Audit's cursor (Day 32), over a different key shape. Used by the validator to check shape (never trusting the value) and by the read service to resume a page and to build the next one's cursor.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentCursor.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 58 lines and 1,859 bytes. Source hash: `cffa8fffc3fcf9d03135b00f4531dc5f04f7d40d1f3f64db38ff72085577cf21`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentCursor.cs#L11): `public static class StudentCursor`
- [Line 15](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentCursor.cs#L15): `public static string Encode(string searchName, Guid id)`
- [Line 24](../../../../../../src/TutoringCentre.Application/Academics/Students/StudentCursor.cs#L24): `public static bool TryDecode(string? cursor, out (string SearchName, Guid Id) value)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs#L13)
- [src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs:46](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L46)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
