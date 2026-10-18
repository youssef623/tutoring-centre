using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers;

/// <summary>
/// Errors shared by the teacher commands and queries. <see cref="PhoneNumber"/> itself has no concept of which
/// form field it fills, so the handler attaches the field here, once, rather than every call site repeating
/// the same dictionary literal.
/// </summary>
internal static class TeacherErrors
{
    public static Error NotFound() => Error.NotFound("teacher.not_found", "This teacher could not be found.");

    public static Error PhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["phone"] = [phoneNumberError.Message] });

    /// <summary>
    /// The one answer for every ineligible userId: unknown, another centre's user, a user with another role,
    /// and an inactive member all look the same here, so the response never discloses which case it was.
    /// </summary>
    public static Error UserNotEligible() => new(
        "teacher.user_not_eligible",
        "This person is not an eligible Teacher at this centre.",
        ErrorKind.Rule,
        new Dictionary<string, string[]> { ["userId"] = ["This person is not an eligible Teacher at this centre."] });

    /// <summary>Matches the Conflict the unique-constraint catalogue reports for ux_teachers_centre_user, so a race loser gets the same answer as the handler's own check.</summary>
    public static Error AccountTaken() => new(
        "teacher.account_taken",
        "This account is already linked to another teacher.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["userId"] = ["This account is already linked to another teacher."] });
}
