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
