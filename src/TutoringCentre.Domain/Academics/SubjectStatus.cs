namespace TutoringCentre.Domain.Academics;

/// <summary>A subject's lifecycle state. Subjects are never deleted, only archived.</summary>
public enum SubjectStatus
{
    Active,
    Archived,
}
