using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant.Tools;

/// <summary>Deliberately small, closed JSON schema dialect. Unknown schema keywords fail registration.</summary>
public sealed class ToolArguments
{
    private readonly JsonElement _value;
    private ToolArguments(JsonElement value) => _value = value;
    public string GetString(string name) => _value.GetProperty(name).GetString()!;
    public int GetInteger(string name) => _value.GetProperty(name).GetInt32();
    public string HandleSummary() => "handles=" + string.Join(',', HandleValues(_value).Distinct(StringComparer.Ordinal));
    private static IEnumerable<string> HandleValues(JsonElement value)
    {
        foreach (var property in value.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                foreach (var handle in HandleValues(property.Value)) yield return handle;
            }
            else if (property.Value.ValueKind == JsonValueKind.String)
            {
                var text = property.Value.GetString()!.Trim().ToLowerInvariant();
                if (Regex.IsMatch(text, "^(me|child_[0-9]{1,6})$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))) yield return text;
            }
        }
    }
    public static Result<ToolArguments> Parse(JsonElement schema, string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 3 });
            return Matches(schema, doc.RootElement, 0) ? Result<ToolArguments>.Success(new(doc.RootElement.Clone())) : Invalid();
        }
        catch (Exception ex) when (ex is JsonException or ArgumentException or InvalidOperationException or RegexMatchTimeoutException or FormatException or OverflowException)
        { return Invalid(); }
    }
    public static void ValidateSchema(JsonElement schema)
    {
        ValidateNode(schema, 0);
        if (schema.GetProperty("type").GetString() != "object") throw new ArgumentException("Tool schema must be an object.");
    }
    private static void ValidateNode(JsonElement schema, int depth)
    {
        if (schema.ValueKind != JsonValueKind.Object) throw new ArgumentException("Invalid schema.");
        var keywords = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in schema.EnumerateObject())
            if (!keywords.Add(property.Name) || property.Name is not ("type" or "properties" or "required" or "additionalProperties" or "maxLength" or "pattern" or "format" or "enum" or "description"))
                throw new ArgumentException("Unsupported schema keyword.");
        var type = schema.GetProperty("type").GetString();
        if (type == "object")
        {
            if (depth > 1 || !schema.TryGetProperty("additionalProperties", out var extra) || extra.ValueKind != JsonValueKind.False)
                throw new ArgumentException("Object schemas must be closed and shallow.");
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var p in schema.GetProperty("properties").EnumerateObject())
            {
                if (!names.Add(p.Name)) throw new ArgumentException("Duplicate schema property.");
                ValidateNode(p.Value, depth + 1);
            }
            if (schema.TryGetProperty("required", out var required))
                foreach (var p in required.EnumerateArray())
                    if (!names.Contains(p.GetString()!)) throw new ArgumentException("Unknown required property.");
        }
        else if (type is not ("string" or "integer" or "date")) throw new ArgumentException("Unsupported schema type.");
        if (schema.TryGetProperty("pattern", out var pattern)) _ = new Regex(pattern.GetString()!, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
    }
    private static bool Matches(JsonElement schema, JsonElement value, int depth)
    {
        if (depth > 2) return false;
        switch (schema.GetProperty("type").GetString())
        {
            case "object":
                if (depth > 1 || value.ValueKind != JsonValueKind.Object) return false;
                var properties = schema.GetProperty("properties");
                var seen = new HashSet<string>(StringComparer.Ordinal);
                foreach (var property in value.EnumerateObject())
                    if (!seen.Add(property.Name) || !properties.TryGetProperty(property.Name, out var definition) || !Matches(definition, property.Value, depth + 1)) return false;
                if (schema.TryGetProperty("required", out var required))
                    foreach (var name in required.EnumerateArray()) if (!seen.Contains(name.GetString()!)) return false;
                break;
            case "integer": if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out _)) return false; break;
            case "string":
            case "date":
                if (value.ValueKind != JsonValueKind.String) return false;
                var text = value.GetString()!;
                if (schema.TryGetProperty("maxLength", out var max) && text.EnumerateRunes().Count() > max.GetInt32()) return false;
                if (schema.TryGetProperty("pattern", out var pattern) && !Regex.IsMatch(text, pattern.GetString()!, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))) return false;
                if ((schema.GetProperty("type").GetString() == "date" || (schema.TryGetProperty("format", out var format) && format.GetString() == "date"))
                    && !DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) return false;
                break;
            default: return false;
        }
        return !schema.TryGetProperty("enum", out var values) || values.EnumerateArray().Any(x => JsonElement.DeepEquals(x, value));
    }
    private static Result<ToolArguments> Invalid() => Result<ToolArguments>.Failure(Error.Validation("tool.arguments_invalid", "Tool arguments do not match the declared schema."));
}
