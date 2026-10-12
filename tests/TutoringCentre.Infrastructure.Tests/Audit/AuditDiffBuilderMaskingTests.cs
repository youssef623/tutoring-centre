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
