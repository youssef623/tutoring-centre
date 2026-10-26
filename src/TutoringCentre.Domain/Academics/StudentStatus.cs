namespace TutoringCentre.Domain.Academics;

/// <summary>A student's lifecycle state. Students are never deleted, only archived.</summary>
public enum StudentStatus
{
    Active,
    Archived,
}
