using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()
    {
        var result = new RestoreTeacherValidator().Validate(new RestoreTeacherCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "TeacherId");
    }

    [Fact]
    public void Validate_ValidId_HasNoErrors()
    {
        var result = new RestoreTeacherValidator().Validate(new RestoreTeacherCommand(Guid.CreateVersion7(), 0));

        Assert.True(result.IsValid);
    }
}
