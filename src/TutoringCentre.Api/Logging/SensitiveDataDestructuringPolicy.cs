using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace TutoringCentre.Api.Logging;

/// <summary>
/// Safety net: when an object is destructured (<c>{@Request}</c>), properties named like Password, Token, Secret,
/// ConnectionString or Phone* are replaced with "***". The primary rule (Day 7) remains: never log request objects.
/// </summary>
public sealed class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    private const string Mask = "***";

    private static readonly string[] SensitiveFragments = ["Password", "Token", "Secret", "ConnectionString"];

    public bool TryDestructure(
        object value,
        ILogEventPropertyValueFactory propertyValueFactory,
        [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(propertyValueFactory);

        result = null;

        // Strings, dictionaries and collections keep Serilog's default handling.
        if (value is IEnumerable)
        {
            return false;
        }

        var properties = value.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
            .ToArray();

        if (!properties.Any(property => IsSensitive(property.Name)))
        {
            return false;
        }

        var members = new List<LogEventProperty>(properties.Length);
        foreach (var property in properties)
        {
            LogEventPropertyValue memberValue = IsSensitive(property.Name)
                ? new ScalarValue(Mask)
                : propertyValueFactory.CreatePropertyValue(property.GetValue(value), destructureObjects: true);

            members.Add(new LogEventProperty(property.Name, memberValue));
        }

        result = new StructureValue(members, value.GetType().Name);
        return true;
    }

    private static bool IsSensitive(string propertyName) =>
        propertyName.StartsWith("Phone", StringComparison.OrdinalIgnoreCase)
        || SensitiveFragments.Any(fragment => propertyName.Contains(fragment, StringComparison.OrdinalIgnoreCase));
}
