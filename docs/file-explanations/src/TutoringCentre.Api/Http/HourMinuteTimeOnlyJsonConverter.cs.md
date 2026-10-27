# `src/TutoringCentre.Api/Http/HourMinuteTimeOnlyJsonConverter.cs`

## Purpose

Source summary (intent, not independent execution proof): A local time of day as exactly "HH:mm" (24-hour), both ways. The framework default writes "HH:mm:ss" and also reads seconds; the API contract has neither. Anything else — seconds, an offset, a UTC instant — fails to read, which the strict-JSON pipeline turns into 400 request.malformed. The OpenAPI schema is unaffected: it is built from the CLR type, so a TimeOnly still documents as a string with the "time" format.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/HourMinuteTimeOnlyJsonConverter.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,400 bytes. Source hash: `e052ca1f9b451c16d8520549eb809fa82b554483e32e29825182edcf4a78e6d4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Text.Json;`
- Line 3: `using System.Text.Json.Serialization;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Api/Http/HourMinuteTimeOnlyJsonConverter.cs#L13): `internal sealed class HourMinuteTimeOnlyJsonConverter : JsonConverter<TimeOnly>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:82](../../../../../src/TutoringCentre.Api/Program.cs#L82)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TutoringCentre.Api.Http;

/// <summary>
/// A local time of day as exactly "HH:mm" (24-hour), both ways. The framework default writes "HH:mm:ss" and also
/// reads seconds; the API contract has neither. Anything else — seconds, an offset, a UTC instant — fails to read,
/// which the strict-JSON pipeline turns into 400 request.malformed. The OpenAPI schema is unaffected: it is built
/// from the CLR type, so a TimeOnly still documents as a string with the "time" format.
/// </summary>
internal sealed class HourMinuteTimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private const string Format = "HH:mm";

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String
            && TimeOnly.TryParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
            return value;
        }

        throw new JsonException("Expected a local time as HH:mm.");
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
