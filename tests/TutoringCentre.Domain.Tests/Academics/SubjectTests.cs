using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class SubjectTests
{
    private static readonly Guid CentreId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()
    {
        // Act
        var result = Subject.Create(CentreId, "  Maths  ");

        // Assert
        Assert.True(result.IsSuccess);
        var subject = result.Value;
        Assert.Equal(SubjectStatus.Active, subject.Status);
        Assert.Equal("Maths", subject.Name);
        Assert.Equal(CentreId, subject.CentreId);
        Assert.NotEqual(Guid.Empty, subject.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Subject.Create(Guid.Empty, "Maths"), "subject.centre_required");

    [Fact]
    public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Subject.Create(CentreId, ""));

    [Fact]
    public void Create_WithWhitespaceOnlyName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Subject.Create(CentreId, "   "));

    [Fact]
    public void Create_With81CharacterName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Subject.Create(CentreId, new string('a', 81)));

    // Pins down the boundary with a literal 80, not Subject.NameMaxLength: if the constant itself were ever
    // changed, this test must notice, not silently track the new value.
    [Fact]
    public void Create_With80CharacterName_Succeeds()
    {
        var name = new string('a', 80);

        var result = Subject.Create(CentreId, name);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(80, Subject.NameMaxLength);
    }

    [Fact]
    public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()
    {
        var result = Subject.Create(CentreId, "  Applied   Maths ");

        Assert.True(result.IsSuccess);
        var subject = result.Value;
        Assert.Equal("Applied Maths", subject.Name);
        Assert.Equal(Subject.Create(CentreId, "applied maths").Value.NormalizedName, subject.NormalizedName);
    }

    [Fact]
    public void Create_WithArabicName_IsAcceptedUnchanged()
    {
        var result = Subject.Create(CentreId, "رياضيات");

        Assert.True(result.IsSuccess);
        Assert.Equal("رياضيات", result.Value.Name);
    }

    [Fact]
    public void Rename_WithValidName_Succeeds()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;

        var result = subject.Rename("Applied Maths");

        Assert.True(result.IsSuccess);
        Assert.Equal("Applied Maths", subject.Name);
    }

    [Fact]
    public void Rename_WithInvalidName_FailsAndLeavesPreviousName()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;

        var result = subject.Rename("");

        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_invalid", result.Error!.Code);
        Assert.Equal("Maths", subject.Name);
    }

    [Fact]
    public void Archive_Twice_ReturnsAlreadyArchived()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;
        Assert.True(subject.Archive().IsSuccess);

        var result = subject.Archive();

        Assert.True(result.IsFailure);
        Assert.Equal("subject.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhenActive_ReturnsNotArchived()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;

        var result = subject.Restore();

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Rename_WhileArchived_ReturnsArchivedReadOnly()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;
        Assert.True(subject.Archive().IsSuccess);

        var result = subject.Rename("Applied Maths");

        Assert.True(result.IsFailure);
        Assert.Equal("subject.archived_read_only", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal("Maths", subject.Name);
    }

    [Fact]
    public void Restore_WhileArchived_ReturnsToActive()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;
        Assert.True(subject.Archive().IsSuccess);

        var result = subject.Restore();

        Assert.True(result.IsSuccess);
        Assert.Equal(SubjectStatus.Active, subject.Status);
    }

    private static void AssertValidationFailure(Result<Subject> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    private static void AssertNameInvalid(Result<Subject> result)
    {
        AssertValidationFailure(result, "subject.name_invalid");
        Assert.NotNull(result.Error!.Fields);
        Assert.Contains("name", result.Error.Fields.Keys);
    }
}
