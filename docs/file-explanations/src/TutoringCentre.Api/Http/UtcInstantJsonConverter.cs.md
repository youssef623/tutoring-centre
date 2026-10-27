# `src/TutoringCentre.Api/Http/UtcInstantJsonConverter.cs`

## Purpose

Source summary (intent, not independent execution proof): Every instant leaves the API as ISO 8601 in UTC with "Z" (Day 53 contract), whatever offset the value carries; the framework default writes the value's own offset ("+00:00" for what PostgreSQL returns). Reading is the framework's own ISO 8601 parsing, unchanged. The OpenAPI schema keeps the "date-time" format through the schema transformer in Program.cs, since a custom converter hides the JSON type from the schema exporter.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/UtcInstantJsonConverter.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,181 bytes. Source hash: `202759ad4ba2a29bda78ad1ebc2b3e3ad572015137e4c0fa607ed46f7c05f72a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Text.Json;`
- Line 3: `using System.Text.Json.Serialization;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Api/Http/UtcInstantJsonConverter.cs#L13): `internal sealed class UtcInstantJsonConverter : JsonConverter<DateTimeOffset>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:83](../../../../../src/TutoringCentre.Api/Program.cs#L83)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TutoringCentre.Api.Http;

/// <summary>
/// Every instant leaves the API as ISO 8601 in UTC with "Z" (Day 53 contract), whatever offset the value carries;
/// the framework default writes the value's own offset ("+00:00" for what PostgreSQL returns). Reading is the
/// framework's own ISO 8601 parsing, unchanged. The OpenAPI schema keeps the "date-time" format through the schema
/// transformer in Program.cs, since a custom converter hides the JSON type from the schema exporter.
/// </summary>
internal sealed class UtcInstantJsonConverter : JsonConverter<DateTimeOffset>
{
    private const string Format = "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'";

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetDateTimeOffset();

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WriteStringValue(value.UtcDateTime.ToString(Format, CultureInfo.InvariantCulture));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
