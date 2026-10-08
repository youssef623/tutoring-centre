using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Maps a unique-constraint name to the Conflict error save-time translation reports for it (Task 25.4). Each
/// entry deliberately matches the code and message a handler's own set-based check already returns when it
/// wins the race — a loser gets an identical outcome, not a different one, through a different path.
/// </summary>
internal static class UniqueConstraintCatalogue
{
    private static readonly Dictionary<string, Func<Error>> Entries = new(StringComparer.Ordinal)
    {
        ["ux_subjects_centre_normalized_name"] = () => new Error(
            "subject.name_taken",
            "A subject with this name already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] }),
        ["ux_centres_slug"] = () => Error.Conflict("centre.slug_taken", "A centre with this slug already exists."),
    };

    /// <summary>The mapped error for a registered constraint, or null when the constraint is not in the catalogue.</summary>
    public static Error? Find(string? constraintName) =>
        constraintName is not null && Entries.TryGetValue(constraintName, out var factory) ? factory() : null;
}
