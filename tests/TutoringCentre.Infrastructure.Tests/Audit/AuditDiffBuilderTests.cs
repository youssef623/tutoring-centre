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
            new AuditPropertyChange("Name", OriginalValue: "Maths", CurrentValue: "Maths", IsModified: false),
            new AuditPropertyChange("Status", OriginalValue: SubjectStatusStub.Active, CurrentValue: SubjectStatusStub.Active, IsModified: false),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, NameAndStatus, changes);

        Assert.Null(document);
    }

    [Fact]
    public void EnumValues_AreFormattedAsTheirLowerCaseName()
    {
        var changes = new[]
        {
            new AuditPropertyChange("Status", OriginalValue: SubjectStatusStub.Active, CurrentValue: SubjectStatusStub.Archived, IsModified: true),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, NameAndStatus, changes);

        Assert.Equal("""{"status":{"before":"active","after":"archived"}}""", document);
    }

    [Fact]
    public void GuidValues_AreFormattedAsStrings()
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var allowed = new HashSet<string>(StringComparer.Ordinal) { "UserId" };
        var changes = new[] { new AuditPropertyChange("UserId", OriginalValue: null, CurrentValue: userId, IsModified: false) };

        var document = AuditDiffBuilder.Build(AuditAction.Created, allowed, changes);

        Assert.Equal($"{{\"userId\":{{\"before\":null,\"after\":\"{userId}\"}}}}", document);
    }

    [Fact]
    public void NullValues_AreWrittenAsJsonNull_NotTheStringNull()
    {
        var changes = new[]
        {
            new AuditPropertyChange("Name", OriginalValue: "Maths", CurrentValue: null, IsModified: true),
        };

        var document = AuditDiffBuilder.Build(AuditAction.Updated, new HashSet<string>(StringComparer.Ordinal) { "Name" }, changes);

        Assert.Equal("""{"name":{"before":"Maths","after":null}}""", document);
    }

    [Fact]
    public void FieldNames_AreCamelCasedFromThePascalCasePropertyName()
    {
        var allowed = new HashSet<string>(StringComparer.Ordinal) { "DefaultLocale" };
        var changes = new[] { new AuditPropertyChange("DefaultLocale", OriginalValue: null, CurrentValue: "en", IsModified: false) };

        var document = AuditDiffBuilder.Build(AuditAction.Created, allowed, changes);

        Assert.Equal("""{"defaultLocale":{"before":null,"after":"en"}}""", document);
    }

    private enum SubjectStatusStub
    {
        Active,
        Archived,
    }
}
