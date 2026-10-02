using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Domain.Common;

/// <summary>
/// An expected business failure. <see cref="Code"/> is a stable, machine-readable contract
/// ("&lt;feature&gt;.&lt;reason&gt;", e.g. "centre.slug_invalid") that the frontend translates;
/// <see cref="Message"/> is for developers and logs.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "C#-only solution; 'Error' is the agreed domain term (it is reserved only in Visual Basic).")]
public sealed record Error(
    string Code,
    string Message,
    ErrorKind Kind,
    IReadOnlyDictionary<string, string[]>? Fields = null)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorKind.Validation);

    public static Error Validation(string code, string message, IReadOnlyDictionary<string, string[]> fields) =>
        new(code, message, ErrorKind.Validation, fields);

    public static Error NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);

    public static Error Rule(string code, string message) => new(code, message, ErrorKind.Rule);

    public static Error Forbidden(string code, string message) => new(code, message, ErrorKind.Forbidden);
}
