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
