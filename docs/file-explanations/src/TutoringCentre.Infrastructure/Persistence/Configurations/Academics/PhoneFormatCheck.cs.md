# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneFormatCheck.cs`

## Purpose

Source summary (intent, not independent execution proof): The database-level backstop for E.164 shape, shared by academics.parents and academics.students (Day 38 contract): looser than PhoneNumber's own validation (no Egyptian-prefix check), since the Domain already fully validates before save — this check only catches a row that reached the column some other way.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneFormatCheck.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 610 bytes. Source hash: `2faead2626c8efb7c5d0000dad55af259e88c48c58e389ee30e668ed6c6f461f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneFormatCheck.cs#L8): `internal static class PhoneFormatCheck`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs:20](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs#L20)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs:29](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L29)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs:32](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L32)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// The database-level backstop for E.164 shape, shared by academics.parents and academics.students (Day 38
/// contract): looser than PhoneNumber's own validation (no Egyptian-prefix check), since the Domain already
/// fully validates before save — this check only catches a row that reached the column some other way.
/// </summary>
internal static class PhoneFormatCheck
{
    public const string Sql = """phone ~ '^\+[1-9][0-9]{7,14}$'""";

    public const string SqlOrNull = $"""phone IS NULL OR {Sql}""";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
