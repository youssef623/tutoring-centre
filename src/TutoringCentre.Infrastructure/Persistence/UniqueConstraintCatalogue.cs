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
        ["ux_memberships_centre_user"] = () => new Error(
            "staff.already_member",
            "This person already has a membership in this centre.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["email"] = ["This person already has a membership in this centre."] }),

        ["ux_parents_centre_phone"] = () => new Error(
            "parent.phone_taken",
            "A parent with this phone number already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["phone"] = ["A parent with this phone number already exists."] }),

        ["ux_student_parents_student_parent"] = () =>
            Error.Conflict("student.parent_already_linked", "This parent is already linked to this student."),

        ["ux_teachers_centre_user"] = () => new Error(
            "teacher.account_taken",
            "This account is already linked to another teacher.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["userId"] = ["This account is already linked to another teacher."] }),

        ["ux_groups_centre_normalized_name"] = () => new Error(
            "group.name_taken",
            "A group with this name already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["name"] = ["A group with this name already exists."] }),
    };

    /// <summary>
    /// Constraints that must never become a Result, even the generic duplicate one: each guards an invariant
    /// the Domain is supposed to make impossible before a save is ever attempted (Task 38.7) — the one-primary-
    /// per-student partial index is in this set, not the Entries map above, deliberately. A violation here is a
    /// bug in that guarantee, not a race the business expects, so it must stay a loud, unhandled exception
    /// rather than a quiet 409 that would hide it.
    /// </summary>
    private static readonly HashSet<string> NeverTranslated = new(StringComparer.Ordinal)
    {
        "ux_student_parents_one_primary",
    };

    /// <summary>The mapped error for a registered constraint, or null when the constraint is not in the catalogue.</summary>
    public static Error? Find(string? constraintName) =>
        constraintName is not null && Entries.TryGetValue(constraintName, out var factory) ? factory() : null;

    public static bool IsDeliberatelyExcluded(string? constraintName) =>
        constraintName is not null && NeverTranslated.Contains(constraintName);
}
