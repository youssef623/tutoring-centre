namespace TutoringCentre.Domain.Academics;

/// <summary>Why an enrollment ended. A transfer ends one enrollment and starts another; it never edits the first.</summary>
public enum EnrollmentEndReason
{
    Withdrawn,
    Transferred,
}
