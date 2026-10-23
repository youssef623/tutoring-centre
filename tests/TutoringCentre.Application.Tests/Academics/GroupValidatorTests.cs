using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>Shape checks only; the real name and capacity rules live in Group and are tested there.</summary>
public sealed class GroupValidatorTests
{
    private static readonly Guid SomeId = Guid.CreateVersion7();

    [Fact]
    public void CreateGroup_Valid_Passes()
    {
        var result = new CreateGroupValidator().Validate(new CreateGroupCommand("Grade 10 A", SomeId, SomeId, 20));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateGroup_EmptyNameAndIds_FailsOnEachField()
    {
        var result = new CreateGroupValidator().Validate(new CreateGroupCommand("", Guid.Empty, Guid.Empty, 20));

        Assert.Equal(
            ["Name", "SubjectId", "TeacherId"],
            result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray());
    }

    [Fact]
    public void CreateGroup_CapacityOutOfRange_IsLeftToTheEntity()
    {
        var result = new CreateGroupValidator().Validate(new CreateGroupCommand("Grade 10 A", SomeId, SomeId, 0));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RenameGroup_EmptyIdAndName_FailsOnBoth()
    {
        var result = new RenameGroupValidator().Validate(new RenameGroupCommand(Guid.Empty, "", 0));

        Assert.Equal(["GroupId", "Name"], result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray());
    }

    [Fact]
    public void RenameGroup_NameOver80_Fails()
    {
        var result = new RenameGroupValidator().Validate(new RenameGroupCommand(SomeId, new string('a', 81), 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void ChangeGroupCapacity_EmptyId_Fails()
    {
        var result = new ChangeGroupCapacityValidator().Validate(new ChangeGroupCapacityCommand(Guid.Empty, 20, 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "GroupId");
    }

    [Fact]
    public void AssignGroupTeacher_EmptyIds_FailsOnBoth()
    {
        var result = new AssignGroupTeacherValidator().Validate(new AssignGroupTeacherCommand(Guid.Empty, Guid.Empty, 0));

        Assert.Equal(["GroupId", "TeacherId"], result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray());
    }

    [Fact]
    public void ArchiveGroup_EmptyId_Fails()
    {
        var result = new ArchiveGroupValidator().Validate(new ArchiveGroupCommand(Guid.Empty, 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "GroupId");
    }

    [Fact]
    public void RestoreGroup_EmptyId_Fails()
    {
        var result = new RestoreGroupValidator().Validate(new RestoreGroupCommand(Guid.Empty, 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "GroupId");
    }
}
