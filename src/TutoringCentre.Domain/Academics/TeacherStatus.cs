namespace TutoringCentre.Domain.Academics;

/// <summary>A teacher's lifecycle state. Teachers are never deleted, only archived.</summary>
public enum TeacherStatus
{
    Active,
    Archived,
}
