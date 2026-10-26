using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>
/// Errors shared by the student commands and queries. <see cref="PhoneNumber"/> itself has no concept of which
/// form field it fills, so the handler attaches the field here, once, rather than every call site repeating
/// the same dictionary literal. The "parent.*" variants exist only for <c>RegisterStudentCommand</c>'s nested
/// new-parent form — <see cref="TutoringCentre.Application.Academics.Parents.ParentErrors"/>'s own phone errors
/// key the field "phone", which is wrong once that phone is a field of the nested parent, not of the command.
/// </summary>
internal static class StudentErrors
{
    public static Error NotFound() => Error.NotFound("student.not_found", "This student could not be found.");

    public static Error PhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["phone"] = [phoneNumberError.Message] });

    public static Error ParentPhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["parent.phone"] = [phoneNumberError.Message] });

    public static Error ParentPhoneTaken() => new(
        "parent.phone_taken",
        "A parent with this phone number already exists.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["parent.phone"] = ["A parent with this phone number already exists."] });
}
