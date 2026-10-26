using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments;

/// <summary>Errors the enrollment handlers raise themselves; the entities raise their own.</summary>
internal static class EnrollmentErrors
{
    /// <summary>The one answer for an unknown ID and another centre's enrollment.</summary>
    public static Error NotFound() => Error.NotFound("enrollment.not_found", "This enrollment could not be found.");

    public static Error StudentArchived() =>
        Error.Rule("enrollment.student_archived", "This student is archived and cannot be enrolled.");

    /// <summary>Matches the Conflict the unique-constraint catalogue reports for ux_enrollments_active_student_group, so a race loser gets the same answer.</summary>
    public static Error AlreadyEnrolled() =>
        Error.Conflict("enrollment.already_enrolled", "This student is already enrolled in this group.");

    public static Error AlreadyEnded() => Error.Rule("enrollment.already_ended", "This enrollment has already ended.");

    public static Error SameGroup() =>
        Error.Rule("enrollment.same_group", "The student is already in this group. Choose a different group.");

    public static Error SubjectMismatch() =>
        Error.Rule("enrollment.subject_mismatch", "A transfer must be to a group of the same subject.");
}
