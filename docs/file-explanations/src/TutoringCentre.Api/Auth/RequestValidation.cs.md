# `src/TutoringCentre.Api/Auth/RequestValidation.cs`

## Purpose

Source summary (intent, not independent execution proof): Turns a FluentValidation result into the same validation.failed shape the dispatcher produces for CQRS requests (Dispatcher.cs), so a non-CQRS request like login fails exactly like every other validation failure.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/RequestValidation.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,274 bytes. Source hash: `040d28a32ca65af2061be8d102690b8f1e79a8c9f0305b5c8827b03de6de9dfb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation.Results;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Api/Auth/RequestValidation.cs#L10): `internal static class RequestValidation`
- [Line 12](../../../../../src/TutoringCentre.Api/Auth/RequestValidation.cs#L12): `public static Error? ToValidationError(ValidationResult result)`
- [Line 31](../../../../../src/TutoringCentre.Api/Auth/RequestValidation.cs#L31): `private static string ToCamelCase(string propertyName) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:91](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L91)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation.Results;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Turns a FluentValidation result into the same validation.failed shape the dispatcher produces for CQRS
/// requests (Dispatcher.cs), so a non-CQRS request like login fails exactly like every other validation failure.
/// </summary>
internal static class RequestValidation
{
    public static Error? ToValidationError(ValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsValid)
        {
            return null;
        }

        var fields = result.Errors
            .GroupBy(failure => ToCamelCase(failure.PropertyName), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);

        return Error.Validation("validation.failed", "One or more fields are invalid.", fields);
    }

    private static string ToCamelCase(string propertyName) =>
        string.Join('.', propertyName.Split('.').Select(segment =>
            segment.Length == 0 ? segment : char.ToLowerInvariant(segment[0]) + segment[1..]));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
