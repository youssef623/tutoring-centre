using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class GroupTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid SubjectId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActive()
    {
        var result = Group.Create(CentreId, "  Advanced Algebra  ", SubjectId, TeacherId, 20);

        Assert.True(result.IsSuccess);
        var group = result.Value;
        Assert.Equal(GroupStatus.Active, group.Status);
        Assert.Equal("Advanced Algebra", group.Name);
        Assert.Equal(CentreId, group.CentreId);
        Assert.Equal(SubjectId, group.SubjectId);
        Assert.Equal(TeacherId, group.TeacherId);
        Assert.Equal(20, group.Capacity);
        Assert.NotEqual(Guid.Empty, group.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Group.Create(Guid.Empty, "Algebra", SubjectId, TeacherId, 20), "group.centre_required");

    [Fact]
    public void Create_WithEmptySubject_ReturnsSubjectRequired() =>
        AssertValidationFailure(Group.Create(CentreId, "Algebra", Guid.Empty, TeacherId, 20), "group.subject_required");

    [Fact]
    public void Create_WithEmptyTeacher_ReturnsTeacherRequired() =>
        AssertValidationFailure(Group.Create(CentreId, "Algebra", SubjectId, Guid.Empty, 20), "group.teacher_required");

    [Fact]
    public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Group.Create(CentreId, "", SubjectId, TeacherId, 20));

    [Fact]
    public void Create_With80CharacterName_Succeeds()
    {
        var name = new string('a', 80);

        var result = Group.Create(CentreId, name, SubjectId, TeacherId, 20);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(80, Group.NameMaxLength);
    }

    [Fact]
    public void Create_With81CharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Group.Create(CentreId, new string('a', 81), SubjectId, TeacherId, 20));

    [Fact]
    public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()
    {
        var result = Group.Create(CentreId, "  Advanced   Algebra ", SubjectId, TeacherId, 20);

        Assert.True(result.IsSuccess);
        var group = result.Value;
        Assert.Equal("Advanced Algebra", group.Name);
        Assert.Equal(Group.Create(CentreId, "advanced algebra", SubjectId, TeacherId, 20).Value.NormalizedName, group.NormalizedName);
    }

    [Fact]
    public void Create_WithCapacityZero_ReturnsCapacityInvalid() =>
        AssertCapacityInvalid(Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 0));

    [Fact]
    public void Create_WithCapacityOne_Succeeds()
    {
        var result = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Capacity);
        Assert.Equal(1, Group.MinCapacity);
    }

    [Fact]
    public void Create_WithCapacity200_Succeeds()
    {
        var result = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 200);

        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.Value.Capacity);
        Assert.Equal(200, Group.MaxCapacity);
    }

    [Fact]
    public void Create_WithCapacity201_ReturnsCapacityInvalid() =>
        AssertCapacityInvalid(Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 201));

    [Fact]
    public void Rename_WithValidName_ChangesNameAndNormalizedName()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.Rename("Applied Algebra");

        Assert.True(result.IsSuccess);
        Assert.Equal("Applied Algebra", group.Name);
        Assert.Equal(SubjectId, group.SubjectId);
    }

    [Fact]
    public void Rename_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.Rename("");

        Assert.True(result.IsFailure);
        Assert.Equal("group.name_invalid", result.Error!.Code);
        Assert.Equal("Algebra", group.Name);
    }

    [Fact]
    public void ChangeCapacity_WithValidValue_Changes()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.ChangeCapacity(30);

        Assert.True(result.IsSuccess);
        Assert.Equal(30, group.Capacity);
    }

    [Fact]
    public void ChangeCapacity_OutOfRange_FailsAndLeavesPreviousCapacityUnchanged()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.ChangeCapacity(201);

        Assert.True(result.IsFailure);
        Assert.Equal("group.capacity_invalid", result.Error!.Code);
        Assert.Equal(20, group.Capacity);
    }

    [Fact]
    public void AssignTeacher_WithADifferentTeacher_Changes()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;
        var newTeacherId = Guid.NewGuid();

        var result = group.AssignTeacher(newTeacherId);

        Assert.True(result.IsSuccess);
        Assert.Equal(newTeacherId, group.TeacherId);
    }

    [Fact]
    public void AssignTeacher_WithTheCurrentTeacher_SucceedsAndChangesNothing()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.AssignTeacher(TeacherId);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherId, group.TeacherId);
    }

    [Fact]
    public void AssignTeacher_WithEmptyId_ReturnsTeacherRequiredAndLeavesTeacherUnchanged()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.AssignTeacher(Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.Equal("group.teacher_required", result.Error!.Code);
        Assert.Equal(TeacherId, group.TeacherId);
    }

    [Fact]
    public void Archive_Twice_ReturnsAlreadyArchived()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;
        Assert.True(group.Archive().IsSuccess);

        var result = group.Archive();

        Assert.True(result.IsFailure);
        Assert.Equal("group.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhenActive_ReturnsNotArchived()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;

        var result = group.Restore();

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhileArchived_ReturnsToActive()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;
        Assert.True(group.Archive().IsSuccess);

        var result = group.Restore();

        Assert.True(result.IsSuccess);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public void Rename_WhileArchived_ReturnsArchivedReadOnlyAndLeavesNameUnchanged()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;
        Assert.True(group.Archive().IsSuccess);

        var result = group.Rename("Applied Algebra");

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal("Algebra", group.Name);
    }

    [Fact]
    public void ChangeCapacity_WhileArchived_ReturnsArchivedReadOnlyAndLeavesCapacityUnchanged()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;
        Assert.True(group.Archive().IsSuccess);

        var result = group.ChangeCapacity(30);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(20, group.Capacity);
    }

    [Fact]
    public void AssignTeacher_WhileArchived_ReturnsArchivedReadOnlyAndLeavesTeacherUnchanged()
    {
        var group = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 20).Value;
        Assert.True(group.Archive().IsSuccess);

        var result = group.AssignTeacher(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(TeacherId, group.TeacherId);
    }

    private static void AssertValidationFailure(Result<Group> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    private static void AssertNameInvalid(Result<Group> result)
    {
        AssertValidationFailure(result, "group.name_invalid");
        Assert.NotNull(result.Error!.Fields);
        Assert.Contains("name", result.Error.Fields.Keys);
    }

    private static void AssertCapacityInvalid(Result<Group> result)
    {
        AssertValidationFailure(result, "group.capacity_invalid");
        Assert.NotNull(result.Error!.Fields);
        Assert.Contains("capacity", result.Error.Fields.Keys);
    }
}
