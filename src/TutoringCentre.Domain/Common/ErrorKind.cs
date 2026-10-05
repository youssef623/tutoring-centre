namespace TutoringCentre.Domain.Common;

/// <summary>
/// Closed set of expected business-failure kinds. The Api maps each kind to exactly one HTTP status (Day 11),
/// so adding a kind is an API-contract change.
/// </summary>
public enum ErrorKind
{
    Validation,
    NotFound,
    Conflict,
    Rule,
    Forbidden,
    Unauthenticated,
}
