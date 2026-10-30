using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Maps a constraint name to the Conflict error save-time translation reports for it: unique constraints since
/// Task 25.4, exclusion constraints since Day 52. Each unique entry deliberately matches the code and message a
/// handler's own set-based check already returns when it wins the race — a loser gets an identical outcome, not
/// a different one, through a different path. Exclusion entries have no handler-side twin: no handler checks for
/// overlaps, so the constraint is the only arbiter.
/// </summary>
internal static class ConstraintErrorCatalog
{
    private static readonly Dictionary<string, Func<Error>> UniqueEntries = new(StringComparer.Ordinal)
    {
        ["ux_outbound_messages_reply"] = () => Error.Conflict("outbound.reply_already_queued", "A reply is already queued for this message."),
        ["ux_conversations_account_portal_user"] = () => Error.Conflict("chat.conversation_already_exists", "This login already has a web conversation."),
        ["ux_portal_links_centre_user"] = () => Error.Conflict("portal.user_already_linked", "This login already has a portal link."),
        ["ux_portal_links_centre_parent"] = () => Error.Conflict("portal.subject_already_linked", "This subject already has a portal account."),
        ["ux_portal_links_centre_student"] = () => Error.Conflict("portal.subject_already_linked", "This subject already has a portal account."),
        ["ux_subjects_centre_normalized_name"] = () => new Error(
            "subject.name_taken",
            "A subject with this name already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] }),
        ["ux_channel_accounts_provider_external"] = () => Error.Conflict("conversations.channel_taken", "This channel account is already registered."),
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

        ["ux_student_cards_active_student"] = () => Error.Conflict("card.concurrent_issue", "Card issuance changed concurrently. Refresh and try again."),
        ["ux_student_cards_centre_token_hash"] = () => Error.Conflict("card.concurrent_issue", "Card issuance changed concurrently. Refresh and try again."),

        ["ux_attendance_records_session_student"] = () =>
            Error.Conflict("attendance.concurrent_mark", "Attendance changed while this record was being marked. Refresh and try again."),

        // The partial index allows one active enrollment per student and group; a race loser gets the same
        // Conflict as the enroll handler's own check (Day 48).
        ["ux_enrollments_active_student_group"] = () =>
            Error.Conflict("enrollment.already_enrolled", "This student is already enrolled in this group."),
    };

    /// <summary>
    /// Constraints that must never become a Result, even the generic duplicate one: each guards an invariant
    /// the Domain is supposed to make impossible before a save is ever attempted (Task 38.7) — the one-primary-
    /// per-student partial index is in this set, not the UniqueEntries map above, deliberately. A violation here
    /// is a bug in that guarantee, not a race the business expects, so it must stay a loud, unhandled exception
    /// rather than a quiet 409 that would hide it.
    /// </summary>
    private static readonly HashSet<string> NeverTranslated = new(StringComparer.Ordinal)
    {
        "ux_student_parents_one_primary",

        // Generation drops dates that already have a session before adding, so a second session for the same
        // rule and date is a bug in that filter, not a business race (Day 52).
        "ux_class_sessions_rule_date",
    };

    /// <summary>
    /// Every exclusion constraint in the schema (ADR 0012). Unlike unique violations there is no generic fallback:
    /// all exclusion constraints are known, so one missing here is a bug and stays an exception.
    /// </summary>
    private static readonly Dictionary<string, Func<Error>> ExclusionEntries = new(StringComparer.Ordinal)
    {
        ["ex_class_sessions_teacher_overlap"] = () =>
            Error.Conflict("schedule.teacher_double_booked", "The teacher already has a session at this time."),
        ["ex_class_sessions_group_overlap"] = () =>
            Error.Conflict("schedule.group_double_booked", "This group already has a session at this time."),
    };

    /// <summary>The mapped error for a registered unique constraint, or null when it is not in the catalogue.</summary>
    public static Error? FindUnique(string? constraintName) =>
        constraintName is not null && UniqueEntries.TryGetValue(constraintName, out var factory) ? factory() : null;

    /// <summary>The mapped error for a registered exclusion constraint, or null when it is not in the catalogue.</summary>
    public static Error? FindExclusion(string? constraintName) =>
        constraintName is not null && ExclusionEntries.TryGetValue(constraintName, out var factory) ? factory() : null;

    public static bool IsDeliberatelyExcluded(string? constraintName) =>
        constraintName is not null && NeverTranslated.Contains(constraintName);
}
