# `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 38.6: pins the masking rule in `AuditDiffBuilder` directly against plain `PhoneNumber` values, no database — whether a field changed is decided before masking ever runs, so two different numbers that happen to share their last four digits are never silently dropped.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 55 lines and 2,516 bytes. Source hash: `c928d18c05982daec9a89c7e4d46f741a61cbea51444b405f8535a06b97d8f98`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.Json.Nodes;`
- Line 2: `using TutoringCentre.Domain.Common;`
- Line 3: `using TutoringCentre.Infrastructure.Audit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L12): `public sealed class AuditDiffBuilderMaskingTests`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L17): `public void Updated_MaskedFieldChanged_RecordsMaskedBeforeAndAfterValues()`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L30): `public void Updated_TwoDifferentNumbersWithTheSameLastFourDigits_StillProducesAnEntry()`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs#L45): `public void Created_MaskedField_RecordsNullBeforeAndMaskedAfterValue()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.Json.Nodes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Audit;

namespace TutoringCentre.Infrastructure.Tests.Audit;

/// <summary>
/// Task 38.6: pins the masking rule in <see cref="AuditDiffBuilder"/> directly against plain
/// <see cref="PhoneNumber"/> values, no database — whether a field changed is decided before masking ever
/// runs, so two different numbers that happen to share their last four digits are never silently dropped.
/// </summary>
public sealed class AuditDiffBuilderMaskingTests
{
    private static readonly HashSet<string> PhoneOnly = new(StringComparer.Ordinal) { "Phone" };

    [Fact]
    public void Updated_MaskedFieldChanged_RecordsMaskedBeforeAndAfterValues()
    {
        var before = PhoneNumber.Create("01012345678").Value;
        var after = PhoneNumber.Create("01112345678").Value;
        var changes = new[] { new AuditPropertyChange("Phone", before, after, IsModified: true) };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, PhoneOnly, changes, PhoneOnly);

        var expected = new JsonObject { ["phone"] = new JsonObject { ["before"] = before.Masked, ["after"] = after.Masked } };
        Assert.Equal(expected.ToJsonString(), document);
    }

    [Fact]
    public void Updated_TwoDifferentNumbersWithTheSameLastFourDigits_StillProducesAnEntry()
    {
        var before = PhoneNumber.Create("01012345678").Value; // +201012345678
        var after = PhoneNumber.Create("01512345678").Value; // +201512345678 — different number, same "5678" tail
        var changes = new[] { new AuditPropertyChange("Phone", before, after, IsModified: true) };

        // Masking alone could never prove these differ — the masked forms below are identical strings.
        Assert.Equal(before.Masked, after.Masked);

        var document = AuditDiffBuilder.Build(AuditAction.Updated, PhoneOnly, changes, PhoneOnly);

        Assert.NotNull(document);
    }

    [Fact]
    public void Created_MaskedField_RecordsNullBeforeAndMaskedAfterValue()
    {
        var after = PhoneNumber.Create("01012345678").Value;
        var changes = new[] { new AuditPropertyChange("Phone", OriginalValue: null, after, IsModified: false) };

        var document = AuditDiffBuilder.Build(AuditAction.Created, PhoneOnly, changes, PhoneOnly);

        var expected = new JsonObject { ["phone"] = new JsonObject { ["before"] = null, ["after"] = after.Masked } };
        Assert.Equal(expected.ToJsonString(), document);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
