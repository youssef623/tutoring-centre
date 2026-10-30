using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Portal;

internal static class PortalErrors
{
    public static Error SubjectNotFound() => Error.NotFound("portal.subject_not_found", "An active portal subject was not found.");
    public static Error SubjectAlreadyLinked() => Error.Conflict("portal.subject_already_linked", "This subject already has a portal account.");
    public static Error UserAlreadyLinked() => Error.Conflict("portal.user_already_linked", "This login already has a portal link.");
    public static Error NotFound() => Error.NotFound("portal.not_found", "The portal account was not found.");
}
