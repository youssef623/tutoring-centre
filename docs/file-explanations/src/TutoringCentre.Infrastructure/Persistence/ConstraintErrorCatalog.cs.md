# `src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs`

## Purpose

The catalogue binds exact database constraint names to public feature errors and field details. It explicitly excludes one-primary-parent and rule/date duplicate constraints because those indicate application invariant failures. Adding or renaming a migration constraint requires reviewing this contract so race losers and friendly pre-checks retain consistent outcomes.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 93 lines and 5,306 bytes. Source hash: `3b13f15d6a733137179593ade1ad80ccf097a32fa795ceb328d1bec490e01348`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs#L12): `internal static class ConstraintErrorCatalog`
- [Line 84](../../../../../src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs#L84): `public static Error? FindUnique(string? constraintName) =>`
- [Line 88](../../../../../src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs#L88): `public static Error? FindExclusion(string? constraintName) =>`
- [Line 91](../../../../../src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs#L91): `public static bool IsDeliberatelyExcluded(string? constraintName) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs:40](../../../../../src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs#L40)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Maps a constraint name to the Conflict error save-time translation reports for it: unique constraints since
/// Task 25.4, exclusion constraints since Day 52. Each unique entry deliberately matches the code and message a
/// handler's own set-based check already returns when it wins the race — a loser gets an identical outcome, not
/// a different one, through a different path. Exclusion entries have no handler-side twin: no handler checks for
/// overlaps, so the constraint is the only arbiter.
/// </summary>
internal static class ConstraintErrorCatalog
{
    private static readonly Dictionary<string, Func<Error>> UniqueEntries = new(StringComparer.Ordinal)
    {
        ["ux_subjects_centre_normalized_name"] = () => new Error(
            "subject.name_taken",
            "A subject with this name already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] }),
        ["ux_centres_slug"] = () => Error.Conflict("centre.slug_taken", "A centre with this slug already exists."),
        ["ux_memberships_centre_user"] = () => new Error(
            "staff.already_member",
            "This person already has a membership in this centre.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["email"] = ["This person already has a membership in this centre."] }),

        ["ux_parents_centre_phone"] = () => new Error(
            "parent.phone_taken",
            "A parent with this phone number already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["phone"] = ["A parent with this phone number already exists."] }),

        ["ux_student_parents_student_parent"] = () =>
            Error.Conflict("student.parent_already_linked", "This parent is already linked to this student."),

        ["ux_teachers_centre_user"] = () => new Error(
            "teacher.account_taken",
            "This account is already linked to another teacher.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["userId"] = ["This account is already linked to another teacher."] }),

        ["ux_groups_centre_normalized_name"] = () => new Error(
            "group.name_taken",
            "A group with this name already exists.",
            ErrorKind.Conflict,
            new Dictionary<string, string[]> { ["name"] = ["A group with this name already exists."] }),

        // The partial index allows one active enrollment per student and group; a race loser gets the same
        // Conflict as the enroll handler's own check (Day 48).
        ["ux_enrollments_active_student_group"] = () =>
            Error.Conflict("enrollment.already_enrolled", "This student is already enrolled in this group."),
    };

    /// <summary>
    /// Constraints that must never become a Result, even the generic duplicate one: each guards an invariant
    /// the Domain is supposed to make impossible before a save is ever attempted (Task 38.7) — the one-primary-
    /// per-student partial index is in this set, not the UniqueEntries map above, deliberately. A violation here
    /// is a bug in that guarantee, not a race the business expects, so it must stay a loud, unhandled exception
    /// rather than a quiet 409 that would hide it.
    /// </summary>
    private static readonly HashSet<string> NeverTranslated = new(StringComparer.Ordinal)
    {
        "ux_student_parents_one_primary",

        // Generation drops dates that already have a session before adding, so a second session for the same
        // rule and date is a bug in that filter, not a business race (Day 52).
        "ux_class_sessions_rule_date",
    };

    /// <summary>
    /// Every exclusion constraint in the schema (ADR 0012). Unlike unique violations there is no generic fallback:
    /// all exclusion constraints are known, so one missing here is a bug and stays an exception.
    /// </summary>
    private static readonly Dictionary<string, Func<Error>> ExclusionEntries = new(StringComparer.Ordinal)
    {
        ["ex_class_sessions_teacher_overlap"] = () =>
            Error.Conflict("schedule.teacher_double_booked", "The teacher already has a session at this time."),
        ["ex_class_sessions_group_overlap"] = () =>
            Error.Conflict("schedule.group_double_booked", "This group already has a session at this time."),
    };

    /// <summary>The mapped error for a registered unique constraint, or null when it is not in the catalogue.</summary>
    public static Error? FindUnique(string? constraintName) =>
        constraintName is not null && UniqueEntries.TryGetValue(constraintName, out var factory) ? factory() : null;

    /// <summary>The mapped error for a registered exclusion constraint, or null when it is not in the catalogue.</summary>
    public static Error? FindExclusion(string? constraintName) =>
        constraintName is not null && ExclusionEntries.TryGetValue(constraintName, out var factory) ? factory() : null;

```

The excerpt stops at line 90; use the original file for the remaining 3 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
