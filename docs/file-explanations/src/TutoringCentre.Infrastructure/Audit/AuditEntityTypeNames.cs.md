# `src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs`

## Purpose

Source summary (intent, not independent execution proof): The one mapping between `AuditEntityType` and its stable string ("subject", "student_parent", …), shared by the database value converter (AuditEntryConfiguration) and the read side's API output (AuditReadService) so the two can never drift apart — needed since Task 38.6's `AuditEntityType.StudentParent` has no simple lower-case mapping from its CLR name alone.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 3,478 bytes. Source hash: `a76cc699cd6ab950e84841ef24e7729930dfab7316db28aa2382bb3ee8def46a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L9): `internal static class AuditEntityTypeNames`
- [Line 11](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L11): `public static string ToDatabase(AuditEntityType value) => value switch`
- [Line 27](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L27): `public static AuditEntityType FromDatabase(string value) => value switch`
- [Line 43](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L43): `public static bool TryParse(string? value, out AuditEntityType result)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:66](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L66)
- [src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs:33](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
        AuditEntityType.Teacher => "teacher",
        AuditEntityType.Group => "group",
        AuditEntityType.Enrollment => "enrollment",
        AuditEntityType.ScheduleRule => "schedule_rule",
        AuditEntityType.ClassSession => "class_session",
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
        "teacher" => AuditEntityType.Teacher,
        "group" => AuditEntityType.Group,
        "enrollment" => AuditEntityType.Enrollment,
        "schedule_rule" => AuditEntityType.ScheduleRule,
        "class_session" => AuditEntityType.ClassSession,
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
            case "teacher":
                result = AuditEntityType.Teacher;
                return true;
            case "group":
                result = AuditEntityType.Group;
                return true;
            case "enrollment":
                result = AuditEntityType.Enrollment;
                return true;
            case "schedule_rule":
                result = AuditEntityType.ScheduleRule;
                return true;
            case "class_session":
                result = AuditEntityType.ClassSession;
                return true;
            default:
                result = default;
                return false;
        }
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
