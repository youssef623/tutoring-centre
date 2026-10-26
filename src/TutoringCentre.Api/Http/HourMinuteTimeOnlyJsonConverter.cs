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
