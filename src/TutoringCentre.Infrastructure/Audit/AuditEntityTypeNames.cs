namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// The one mapping between <see cref="AuditEntityType"/> and its stable string ("subject", "student_parent",
/// …), shared by the database value converter (<c>AuditEntryConfiguration</c>) and the read side's API
/// output (<c>AuditReadService</c>) so the two can never drift apart — needed since Task 38.6's
/// <see cref="AuditEntityType.StudentParent"/> has no simple lower-case mapping from its CLR name alone.
/// </summary>
internal static class AuditEntityTypeNames
{
    public static string ToDatabase(AuditEntityType value) => value switch
    {
        AuditEntityType.Subject => "subject",
        AuditEntityType.Membership => "membership",
        AuditEntityType.Centre => "centre",
        AuditEntityType.Parent => "parent",
        AuditEntityType.Student => "student",
        AuditEntityType.StudentParent => "student_parent",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported audit entity type."),
    };

    public static AuditEntityType FromDatabase(string value) => value switch
    {
        "subject" => AuditEntityType.Subject,
        "membership" => AuditEntityType.Membership,
        "centre" => AuditEntityType.Centre,
        "parent" => AuditEntityType.Parent,
        "student" => AuditEntityType.Student,
        "student_parent" => AuditEntityType.StudentParent,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported entity type value in the database."),
    };

    public static bool TryParse(string? value, out AuditEntityType result)
    {
        switch (value)
        {
            case "subject":
                result = AuditEntityType.Subject;
                return true;
            case "membership":
                result = AuditEntityType.Membership;
                return true;
            case "centre":
                result = AuditEntityType.Centre;
                return true;
            case "parent":
                result = AuditEntityType.Parent;
                return true;
            case "student":
                result = AuditEntityType.Student;
                return true;
            case "student_parent":
                result = AuditEntityType.StudentParent;
                return true;
            default:
                result = default;
                return false;
        }
    }
}
