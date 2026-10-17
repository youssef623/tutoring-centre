using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()
    {
        var result = new UpdateTeacherValidator().Validate(new UpdateTeacherCommand(Guid.Empty, "Amina Hassan", null, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "TeacherId");
    }

    [Fact]
    public void Validate_EmptyName_ReportsErrorOnFullName()
    {
        var result = new UpdateTeacherValidator().Validate(new UpdateTeacherCommand(Guid.CreateVersion7(), "", null, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_Valid_HasNoErrors()
    {
        var result = new UpdateTeacherValidator().Validate(new UpdateTeacherCommand(Guid.CreateVersion7(), "Amina Hassan", null, 0));

        Assert.True(result.IsValid);
    }
}
