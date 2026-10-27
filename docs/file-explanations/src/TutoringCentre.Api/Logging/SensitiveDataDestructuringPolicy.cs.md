# `src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs`

## Purpose

Source summary (intent, not independent execution proof): Safety net: when an object is destructured ({@Request}), properties named like Password, Token, Secret, ConnectionString or Phone* are replaced with "***". The primary rule (Day 7) remains: never log request objects.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 2,286 bytes. Source hash: `7f1b06fdf9f5c360f8563796c8b36bd8a3f1863b0e860dac8238f9650f1cb394`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Collections;`
- Line 2: `using System.Diagnostics.CodeAnalysis;`
- Line 3: `using System.Reflection;`
- Line 4: `using Serilog.Core;`
- Line 5: `using Serilog.Events;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs#L13): `public sealed class SensitiveDataDestructuringPolicy : IDestructuringPolicy`
- [Line 19](../../../../../src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs#L19): `public bool TryDestructure(`
- [Line 59](../../../../../src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs#L59): `private static bool IsSensitive(string propertyName) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:45](../../../../../src/TutoringCentre.Api/Program.cs#L45)
- [tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs:44](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
