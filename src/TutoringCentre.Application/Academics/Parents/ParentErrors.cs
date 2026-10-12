using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents;

/// <summary>
/// Errors shared by the parent commands and queries. <see cref="PhoneNumber"/> itself has no concept of which
/// form field it fills — <c>phone.invalid</c> reaches the handler with no field attached — so the handler
/// attaches "phone" here, once, rather than every call site repeating the same dictionary literal.
/// </summary>
internal static class ParentErrors
{
    public static Error PhoneInvalid(Error phoneNumberError) => new(
        phoneNumberError.Code,
        phoneNumberError.Message,
        phoneNumberError.Kind,
        new Dictionary<string, string[]> { ["phone"] = [phoneNumberError.Message] });

    public static Error PhoneTaken() => new(
        "parent.phone_taken",
        "A parent with this phone number already exists.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["phone"] = ["A parent with this phone number already exists."] });

    public static Error NotFound() => Error.NotFound("parent.not_found", "This parent could not be found.");
}
