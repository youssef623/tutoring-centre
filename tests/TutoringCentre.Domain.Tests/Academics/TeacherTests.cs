using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class TeacherTests
{
    private static readonly Guid CentreId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()
    {
        var result = Teacher.Create(CentreId, "  Nadia Fathy  ", null);

        Assert.True(result.IsSuccess);
        var teacher = result.Value;
        Assert.Equal(TeacherStatus.Active, teacher.Status);
        Assert.Equal("Nadia Fathy", teacher.FullName);
        Assert.Equal(CentreId, teacher.CentreId);
        Assert.NotEqual(Guid.Empty, teacher.Id);
        Assert.Null(teacher.UserId);
        Assert.Null(teacher.Phone);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Teacher.Create(Guid.Empty, "Nadia Fathy", null), "teacher.centre_required");

    [Fact]
    public void Create_WithEmptyName_ReturnsNameInvalidWithFullNameField() =>
        AssertNameInvalid(Teacher.Create(CentreId, "", null));

    [Fact]
    public void Create_WithOneCharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Teacher.Create(CentreId, "A", null));

    [Fact]
    public void Create_With121CharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Teacher.Create(CentreId, new string('a', 121), null));

    [Fact]
    public void Create_With2CharacterName_Succeeds()
    {
        var result = Teacher.Create(CentreId, "Al", null);

        Assert.True(result.IsSuccess);
        Assert.Equal("Al", result.Value.FullName);
        Assert.Equal(2, Teacher.NameMinLength);
    }

    // Pins down the boundary with a literal 120, not Teacher.NameMaxLength: if the constant itself were ever
    // changed, this test must notice, not silently track the new value.
    [Fact]
    public void Create_With120CharacterName_Succeeds()
    {
        var name = new string('a', 120);

        var result = Teacher.Create(CentreId, name, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.FullName);
        Assert.Equal(120, Teacher.NameMaxLength);
    }

    [Fact]
    public void Create_CollapsesInnerWhitespaceAndDerivesMatchingSearchName()
    {
        var result = Teacher.Create(CentreId, "  Nadia   Fathy ", null);

        Assert.True(result.IsSuccess);
        var teacher = result.Value;
        Assert.Equal("Nadia Fathy", teacher.FullName);
        Assert.Equal(Teacher.Create(CentreId, "nadia fathy", null).Value.SearchName, teacher.SearchName);
    }

    [Fact]
    public void UpdateDetails_WithValidName_ChangesNameAndSearchName()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;

        var result = teacher.UpdateDetails("Nadia Samir", null);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nadia Samir", teacher.FullName);
        Assert.Equal(Teacher.Create(CentreId, "nadia samir", null).Value.SearchName, teacher.SearchName);
    }

    [Fact]
    public void UpdateDetails_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;

        var result = teacher.UpdateDetails("", null);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.name_invalid", result.Error!.Code);
        Assert.Equal("Nadia Fathy", teacher.FullName);
    }

    [Fact]
    public void LinkUser_WithEmptyId_ReturnsUserRequiredAndLeavesLinkUnchanged()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;

        var result = teacher.LinkUser(Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.user_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public void LinkUser_WithAnId_SetsUserId()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        var userId = Guid.NewGuid();

        var result = teacher.LinkUser(userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public void LinkUser_WithTheAlreadyLinkedUser_SucceedsAndChangesNothing()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        var userId = Guid.NewGuid();
        Assert.True(teacher.LinkUser(userId).IsSuccess);

        var result = teacher.LinkUser(userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public void UnlinkUser_WhenLinked_ClearsTheLink()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        Assert.True(teacher.LinkUser(Guid.NewGuid()).IsSuccess);

        var result = teacher.UnlinkUser();

        Assert.True(result.IsSuccess);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public void Archive_Twice_ReturnsAlreadyArchived()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        Assert.True(teacher.Archive().IsSuccess);

        var result = teacher.Archive();

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhenActive_ReturnsNotArchived()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;

        var result = teacher.Restore();

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhileArchived_ReturnsToActive()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        Assert.True(teacher.Archive().IsSuccess);

        var result = teacher.Restore();

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherStatus.Active, teacher.Status);
    }

    [Fact]
    public void Archive_KeepsTheAccountLink()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        var userId = Guid.NewGuid();
        Assert.True(teacher.LinkUser(userId).IsSuccess);

        Assert.True(teacher.Archive().IsSuccess);

        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public void UpdateDetails_WhileArchived_ReturnsArchivedReadOnlyAndLeavesNameUnchanged()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        Assert.True(teacher.Archive().IsSuccess);

        var result = teacher.UpdateDetails("Nadia Samir", null);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.archived_read_only", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal("Nadia Fathy", teacher.FullName);
    }

    [Fact]
    public void LinkUser_WhileArchived_ReturnsArchivedReadOnlyAndLeavesLinkUnchanged()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        Assert.True(teacher.Archive().IsSuccess);

        var result = teacher.LinkUser(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.archived_read_only", result.Error!.Code);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public void UnlinkUser_WhileArchived_ReturnsArchivedReadOnlyAndLeavesLinkUnchanged()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;
        var userId = Guid.NewGuid();
        Assert.True(teacher.LinkUser(userId).IsSuccess);
        Assert.True(teacher.Archive().IsSuccess);

        var result = teacher.UnlinkUser();

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.archived_read_only", result.Error!.Code);
        Assert.Equal(userId, teacher.UserId);
    }

    private static void AssertValidationFailure(Result<Teacher> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    private static void AssertNameInvalid(Result<Teacher> result)
    {
        AssertValidationFailure(result, "teacher.name_invalid");
        Assert.NotNull(result.Error!.Fields);
        Assert.Contains("fullName", result.Error.Fields.Keys);
    }
}
