# `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 31.5: pure tests, no database — exercises every rule in the Day 31 contract's "Changes document" section directly against plain values.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 170 lines and 6,678 bytes. Source hash: `c5c2825afe2033e2be0e156df70f5ff39ca8a6d058581d2f6afd838634acdc50`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Infrastructure.Audit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L7): `public sealed class AuditDiffBuilderTests`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L12): `public void Created_IncludesEveryAllowedField_WithNullBeforeAndTheValueAfter()`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L28): `public void Updated_IncludesOnlyModifiedAllowedFieldsThatActuallyDiffer()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L42): `public void Deleted_IncludesEveryAllowedField_WithTheValueBeforeAndNullAfter()`
- [Line 58](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L58): `public void Updated_AModifiedButEqualValue_IsSkipped()`
- [Line 71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L71): `public void Updated_ANonAllowedField_IsNeverPresentEvenWhenModified()`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L87): `public void Updated_WithNoAllowedFieldReallyChanged_YieldsNull()`
- [Line 101](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L101): `public void EnumValues_AreFormattedAsTheirLowerCaseName()`
- [Line 114](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L114): `public void GuidValues_AreFormattedAsStrings()`
- [Line 126](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L126): `public void NullValues_AreWrittenAsJsonNull_NotTheStringNull()`
- [Line 139](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L139): `public void FieldNames_AreCamelCasedFromThePascalCasePropertyName()`
- [Line 150](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L150): `public void Created_WritesDatesAndTimesAsInvariantIsoText()`
- [Line 165](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs#L165): `private enum SubjectStatusStub`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Infrastructure.Audit;

namespace TutoringCentre.Infrastructure.Tests.Audit;

/// <summary>Task 31.5: pure tests, no database — exercises every rule in the Day 31 contract's "Changes
/// document" section directly against plain values.</summary>
public sealed class AuditDiffBuilderTests
{
    private static readonly HashSet<string> NameAndStatus = new(StringComparer.Ordinal) { "Name", "Status" };

    [Fact]
    public void Created_IncludesEveryAllowedField_WithNullBeforeAndTheValueAfter()
    {
        var changes = new[]
        {
            new AuditPropertyChange("Name", OriginalValue: null, CurrentValue: "Maths", IsModified: false),
            new AuditPropertyChange("Status", OriginalValue: null, CurrentValue: SubjectStatusStub.Active, IsModified: false),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Created, NameAndStatus, changes);

        Assert.Equal(
            """{"name":{"before":null,"after":"Maths"},"status":{"before":null,"after":"active"}}""",
            document);
    }

    [Fact]
    public void Updated_IncludesOnlyModifiedAllowedFieldsThatActuallyDiffer()
    {
        var changes = new[]
        {
            new AuditPropertyChange("Name", OriginalValue: "Maths", CurrentValue: "Mathematics", IsModified: true),
            new AuditPropertyChange("Status", OriginalValue: SubjectStatusStub.Active, CurrentValue: SubjectStatusStub.Active, IsModified: false),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, NameAndStatus, changes);

        Assert.Equal("""{"name":{"before":"Maths","after":"Mathematics"}}""", document);
    }

    [Fact]
    public void Deleted_IncludesEveryAllowedField_WithTheValueBeforeAndNullAfter()
    {
        var changes = new[]
        {
            new AuditPropertyChange("Name", OriginalValue: "Maths", CurrentValue: null, IsModified: false),
            new AuditPropertyChange("Status", OriginalValue: SubjectStatusStub.Active, CurrentValue: null, IsModified: false),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Deleted, NameAndStatus, changes);

        Assert.Equal(
            """{"name":{"before":"Maths","after":null},"status":{"before":"active","after":null}}""",
            document);
    }

    [Fact]
    public void Updated_AModifiedButEqualValue_IsSkipped()
    {
        var changes = new[]
        {
            new AuditPropertyChange("Name", OriginalValue: "Maths", CurrentValue: "Maths", IsModified: true),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, NameAndStatus, changes);

        Assert.Null(document);
    }

    [Fact]
    public void Updated_ANonAllowedField_IsNeverPresentEvenWhenModified()
    {
        var changes = new[]
        {
            new AuditPropertyChange("NormalizedName", OriginalValue: "MATHS", CurrentValue: "MATHEMATICS", IsModified: true),
            new AuditPropertyChange("Name", OriginalValue: "Maths", CurrentValue: "Mathematics", IsModified: true),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, NameAndStatus, changes);

        Assert.NotNull(document);
        Assert.DoesNotContain("normalizedName", document);
        Assert.DoesNotContain("NormalizedName", document);
    }

    [Fact]
    public void Updated_WithNoAllowedFieldReallyChanged_YieldsNull()
    {
        var changes = new[]
        {
```

The excerpt stops at line 90; use the original file for the remaining 80 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
