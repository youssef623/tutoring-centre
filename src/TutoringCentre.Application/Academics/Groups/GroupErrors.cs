using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups;

/// <summary>Errors the group handlers raise themselves; the entity raises its own.</summary>
internal static class GroupErrors
{
    /// <summary>The one answer for an unknown ID, another centre's group and a group outside the actor's teacher scope.</summary>
    public static Error NotFound() => Error.NotFound("group.not_found", "This group could not be found.");

    /// <summary>Matches the Conflict the unique-constraint catalogue reports for ux_groups_centre_normalized_name, so a race loser gets the same answer.</summary>
    public static Error NameTaken() => new(
        "group.name_taken",
        "A group with this name already exists.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["name"] = ["A group with this name already exists."] });

    /// <summary>Matches the Conflict the save reports for a stale row version, for the locked loads that compare it in the handler.</summary>
    public static Error Stale() =>
        Error.Conflict("concurrency.stale", "This record was changed by someone else. Reload and try again.");

    public static Error HasActiveEnrollments() =>
        Error.Rule("group.has_active_enrollments", "This group still has enrolled students.");

    public static Error HasActiveSchedule() =>
        Error.Rule("group.has_active_schedule", "This group still has a schedule rule in effect. End it first.");

    public static Error SubjectArchived() => Error.Rule("group.subject_archived", "This subject is archived.");

    public static Error TeacherArchived() => Error.Rule("group.teacher_archived", "This teacher is archived.");

    public static Error SubjectNotFound() => Error.NotFound("subject.not_found", "This subject could not be found.");

    public static Error TeacherNotFound() => Error.NotFound("teacher.not_found", "This teacher could not be found.");
}
