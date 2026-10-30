using TutoringCentre.Application.Identity.Portal;

namespace TutoringCentre.Application.Identity.Queries.ValidateStaffSession;

public readonly record struct SessionValidationResult(bool IsValid, PortalSubject? PortalSubject = null)
{
    // Preserve existing boolean consumers while carrying the server-resolved subject.
    public bool ToBoolean() => IsValid;
    public static implicit operator bool(SessionValidationResult result) => result.IsValid;
}
